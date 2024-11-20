
using System;

namespace CommandPatternDemo.Core.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string CommandType { get; set; }
        public string CommandData { get; set; }
        public string UserId { get; set; }
        public DateTime ExecutedAt { get; set; }
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
    }
}