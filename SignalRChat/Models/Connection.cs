using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRChat.Models
{
    public class Connection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string ConnectionID { get; set; }
        public string UserAgent { get; set; }
        public ConnectionType Connected { get; set; }
        public string UserName { get; set; }
    }
}