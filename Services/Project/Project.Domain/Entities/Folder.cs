﻿namespace Project.Domain.Entities
{
    public class Folder : BaseEntity
    {
        public string Name { get; set; } = default!;
        public int? ProjectId { get; set; }
        public int Version { get; set; } = 0; //Tối đa 10 version
        public int ParentId { get; set; } = default!;  //Nếu parent id bằng 0 thì tức là nó không có folder cha
        public bool IsCheckin { get; set; }
        public bool IsCheckout { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string FullPath { get; set; } = string.Empty;
        public string FullPathName { get; set; } = string.Empty;
        public bool IsShow { get; set; } = false;
        public Access Access { get; set; } = Access.Write;
        public ICollection<FolderTag>? FolderTags { get; set; }
        public ICollection<FolderHistory>? FolderHistories { get; set; }
        public ICollection<FolderPermission>? FolderPermissions { get; set; }

    }
}
