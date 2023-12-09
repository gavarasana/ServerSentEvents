using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hobron.SSE.Worker.Dto
{
    public class Notification
    {
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime MessageTime { get; set; } = DateTime.Now;
        public bool IsProcessed { get; set; } = false;
    }
}
