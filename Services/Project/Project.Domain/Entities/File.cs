﻿namespace Project.Domain.Entities
{
    public class File : BaseEntity
    {
        public string Name { get; set; } = default!;
        public decimal Size { get; set; }
        public string Url { get; set; } = default!;
        public int? FolderId { get; set; } 
        public int? ProjectId { get; set; }
        public int FileVersion { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsCheckIn { get; set; }
        public bool IsCheckout { get; set; } = true;
        public string FullPath { get; set; } = string.Empty;
        public FileType FileType { get; set; } = default!;
        public string MimeType { get; set; } = default!;
        public string Extension { get; set; } = default!;
        public Access Access { get; set; } = Access.Write;
        public bool IsShow { get; set; } = false;
        public ICollection<FileTag>? FileTags { get; set; }
        public ICollection<FilePermission>? FilePermissions { get; set; }

    }

}
