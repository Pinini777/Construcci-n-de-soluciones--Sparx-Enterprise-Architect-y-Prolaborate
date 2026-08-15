using System;

namespace Addino
{
    public class MetadataElementRow
    {
        public MetadataElementRow(
            int elementId,
            string name,
            string alias,
            string notes,
            string type,
            string stereotype)
            : this(elementId, name, alias, notes, type, stereotype, string.Empty)
        {
        }

        public MetadataElementRow(
            int elementId,
            string name,
            string alias,
            string notes,
            string type,
            string stereotype,
            string packagePath)
        {
            ElementId = elementId;
            Name = name ?? string.Empty;
            Alias = alias ?? string.Empty;
            Notes = notes ?? string.Empty;
            Type = type ?? string.Empty;
            Stereotype = stereotype ?? string.Empty;
            PackagePath = packagePath ?? string.Empty;

            OriginalName = Name;
            OriginalAlias = Alias;
            OriginalNotes = Notes;
        }

        public int ElementId { get; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string Notes { get; set; }

        public string Type { get; }

        public string Stereotype { get; }

        public string PackagePath { get; }

        public string OriginalName { get; private set; }

        public string OriginalAlias { get; private set; }

        public string OriginalNotes { get; private set; }

        public bool IsDirty
        {
            get
            {
                return
                    !string.Equals(Name, OriginalName, StringComparison.Ordinal) ||
                    !string.Equals(Alias, OriginalAlias, StringComparison.Ordinal) ||
                    !string.Equals(Notes, OriginalNotes, StringComparison.Ordinal);
            }
        }

        public void AcceptChanges()
        {
            OriginalName = Name ?? string.Empty;
            OriginalAlias = Alias ?? string.Empty;
            OriginalNotes = Notes ?? string.Empty;
        }
    }
}
