using System;

namespace FunctionApp.Models
{
    public class TDocument
    {
        public TDocument(string fileName, string fileExtension)
        {
            FileName = fileName;
            FileExtension = fileExtension;
        }

        public string DocumentNode { get; set; }
        public short? DocumentLevel { get; set; }
        public string Title { get; set; }
        public int Owner { get; set; }
        public bool FolderFlag { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string Revision { get; set; }
        public int ChangeNumber { get; set; }
        public byte Status { get; set; }
        public string DocumentSummary { get; set; }
        public byte[] Document { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
