using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class Post
    {
        public Post()
        {
            WorkersPosts = new HashSet<WorkersPost>();
        }

        public int IdPost { get; set; }
        public string NamePost { get; set; } = null!;
        public double Salary { get; set; }
        public double PayForDay { get; set; }

        public virtual ICollection<WorkersPost> WorkersPosts { get; set; }
    }
}
