using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Addino
{
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
                rows = LoadPackageElements(package, out warnings);
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

            // Phase 2: abrir MetadataReviewForm con repository y rows.
            // La colección está completamente preparada localmente para la siguiente unidad de trabajo.
        }


        private BindingList<MetadataElementRow> LoadPackageElements(
            EA.Package package,
            out List<string> warnings)
        {
            warnings = new List<string>();
            BindingList<MetadataElementRow> rows = new BindingList<MetadataElementRow>();

            if (package == null)
            {
                return rows;
            }

            EA.Collection elements;

            try
            {
                elements = package.Elements;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "No se pudo acceder a la colección de elementos del paquete.",
                    ex);
            }

            if (elements == null)
            {
                return rows;
            }

            try
            {
                foreach (object elementObject in elements)
                {
                    EA.Element element = elementObject as EA.Element;
                    if (element == null)
                    {
                        continue;
                    }

                    try
                    {
                        rows.Add(new MetadataElementRow(
                            element.ElementID,
                            element.Name,
                            element.Alias,
                            element.Notes,
                            element.Type,
                            element.Stereotype));
                    }
                    catch (Exception ex)
                    {
                        warnings.Add($"Elemento ID {element.ElementID}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "No se pudo recorrer la colección de elementos del paquete.",
                    ex);
            }

            return rows;
        }


        public void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
