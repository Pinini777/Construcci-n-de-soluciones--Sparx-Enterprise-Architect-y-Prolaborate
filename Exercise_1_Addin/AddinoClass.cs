using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Addino
{
    internal delegate BindingList<MetadataElementRow> PackageLoader(
        EA.Package root,
        out List<string> warnings);

    public class AddinoClass
    {
        private const string MenuHeader = "-&Addino";
        private const string MenuReview = "&Revisión de Metadatos de Elementos";


        public string EA_Connect(EA.Repository repository)
        {
            return "Addino conectado correctamente";
        }


        public object EA_GetMenuItems(
            EA.Repository repository,
            string location,
            string menuName)
        {
            switch (menuName)
            {
                case "":
                    return MenuHeader;

                case MenuHeader:
                    return new string[]
                    {
                        MenuReview
                    };
            }

            return "";
        }


        public void EA_GetMenuState(
            EA.Repository repository,
            string location,
            string menuName,
            string itemName,
            ref bool isEnabled,
            ref bool isChecked)
        {
            isEnabled = true;
        }


        public void EA_MenuClick(
            EA.Repository repository,
            string location,
            string menuName,
            string itemName)
        {
            if (itemName != MenuReview)
            {
                return;
            }

            if (repository.GetTreeSelectedItemType() != EA.ObjectType.otPackage)
            {
                MessageBox.Show(
                    "Seleccione un paquete en el Project Browser para revisar sus elementos.",
                    "Addino",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            object selectedObject = repository.GetTreeSelectedObject();
            if (selectedObject == null)
            {
                MessageBox.Show(
                    "No se pudo obtener el objeto seleccionado. Seleccione un paquete válido.",
                    "Addino",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            EA.Package package = selectedObject as EA.Package;
            if (package == null)
            {
                MessageBox.Show(
                    "El objeto seleccionado no es un paquete válido.",
                    "Addino",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            BindingList<MetadataElementRow> rows;
            List<string> warnings;

            try
            {
                rows = LoadPackageTree(package, out warnings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar los elementos del paquete: {ex.Message}",
                    "Addino",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (warnings.Count > 0)
            {
                string details = string.Join(Environment.NewLine, warnings);

                MessageBox.Show(
                    $"Algunos elementos no pudieron leerse y fueron omitidos:{Environment.NewLine}{details}",
                    "Addino",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            using (MetadataReviewForm form = new MetadataReviewForm(
                repository,
                package,
                LoadPackageTree,
                rows))
            {
                form.ShowDialog();
            }
        }


        internal static BindingList<MetadataElementRow> LoadPackageTree(
            EA.Package root,
            out List<string> warnings)
        {
            warnings = new List<string>();
            BindingList<MetadataElementRow> rows = new BindingList<MetadataElementRow>();

            if (root == null)
            {
                return rows;
            }

            HashSet<int> visitedPackageIds = new HashSet<int>();
            HashSet<int> emittedElementIds = new HashSet<int>();
            Stack<PackageFrame> stack = new Stack<PackageFrame>();

            stack.Push(new PackageFrame
            {
                Package = root,
                Path = SafePackageName(root)
            });

            while (stack.Count > 0)
            {
                PackageFrame frame = stack.Pop();
                EA.Package package = frame.Package;
                string path = frame.Path;

                if (package == null)
                {
                    continue;
                }

                int packageId;

                try
                {
                    packageId = package.PackageID;
                }
                catch (Exception ex)
                {
                    warnings.Add(
                        $"No se pudo leer el identificador de un paquete en la rama '{path}': {ex.Message}; se omite la rama.");

                    continue;
                }

                if (visitedPackageIds.Contains(packageId))
                {
                    warnings.Add(
                        $"Paquete ID {packageId} ('{path}') ya fue procesado; se omite la rama para evitar ciclos.");

                    continue;
                }

                visitedPackageIds.Add(packageId);

                EA.Collection elements = null;

                try
                {
                    elements = package.Elements;
                }
                catch (Exception ex)
                {
                    warnings.Add(
                        $"No se pudieron leer los elementos del paquete '{path}' (ID {packageId}): {ex.Message}; se omite la rama.");

                    continue;
                }

                if (elements != null)
                {
                    foreach (object elementObject in elements)
                    {
                        EA.Element element = null;

                        try
                        {
                            element = elementObject as EA.Element;
                        }
                        catch
                        {
                            // Ignore cast failures and continue with the next item.
                        }

                        if (element == null)
                        {
                            continue;
                        }

                        int elementId;
                        string elementName;
                        string elementAlias;
                        string elementNotes;
                        string elementType;
                        string elementStereotype;

                        try
                        {
                            elementId = element.ElementID;
                            elementName = element.Name;
                            elementAlias = element.Alias;
                            elementNotes = element.Notes;
                            elementType = element.Type;
                            elementStereotype = element.Stereotype;
                        }
                        catch (Exception ex)
                        {
                            warnings.Add(
                                $"Elemento en paquete '{path}' (ID {packageId}): no se pudieron leer sus datos; se omite ({ex.Message}).");

                            continue;
                        }

                        if (emittedElementIds.Contains(elementId))
                        {
                            continue;
                        }

                        emittedElementIds.Add(elementId);

                        rows.Add(new MetadataElementRow(
                            elementId,
                            elementName ?? string.Empty,
                            elementAlias ?? string.Empty,
                            elementNotes ?? string.Empty,
                            elementType ?? string.Empty,
                            elementStereotype ?? string.Empty,
                            path));
                    }
                }

                EA.Collection childPackages = null;

                try
                {
                    childPackages = package.Packages;
                }
                catch (Exception ex)
                {
                    warnings.Add(
                        $"No se pudieron leer los subpaquetes de '{path}' (ID {packageId}): {ex.Message}; se omite la rama.");

                    continue;
                }

                if (childPackages == null)
                {
                    continue;
                }

                List<EA.Package> children = new List<EA.Package>();

                foreach (object childObject in childPackages)
                {
                    EA.Package child = null;

                    try
                    {
                        child = childObject as EA.Package;
                    }
                    catch
                    {
                        // Ignore cast failures.
                    }

                    if (child != null)
                    {
                        children.Add(child);
                    }
                }

                for (int i = children.Count - 1; i >= 0; i--)
                {
                    EA.Package child = children[i];
                    string childPath = BuildChildPath(path, SafePackageName(child));

                    stack.Push(new PackageFrame
                    {
                        Package = child,
                        Path = childPath
                    });
                }
            }

            return rows;
        }


        private static string SafePackageName(EA.Package package)
        {
            if (package == null)
            {
                return string.Empty;
            }

            try
            {
                return package.Name ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }


        private static string BuildChildPath(string parentPath, string childName)
        {
            if (string.IsNullOrEmpty(parentPath))
            {
                return childName ?? string.Empty;
            }

            if (string.IsNullOrEmpty(childName))
            {
                return parentPath;
            }

            return parentPath + " / " + childName;
        }


        private struct PackageFrame
        {
            public EA.Package Package;
            public string Path;
        }


        public void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
