//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShortHorn.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TodoList
    {
        public TodoList()
        {
            this.IsFavourite = false;
            this.Items = new HashSet<TodoItem>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsFavourite { get; set; }
        public int UserId { get; set; }
    
        public virtual ICollection<TodoItem> Items { get; set; }
        public virtual User Owner { get; set; }
    }
}
