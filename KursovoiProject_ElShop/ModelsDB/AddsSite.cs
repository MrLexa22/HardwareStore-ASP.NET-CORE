using System;
using System.Collections.Generic;

namespace KursovoiProject_ElShop
{
    public partial class AddsSite
    {
        public int IdAdds { get; set; }
        public string Name { get; set; } = null!;
        public int TypeWhere { get; set; }
        public string FtppathImage { get; set; } = null!;
        public string Href { get; set; } = null!;
        public DateTime? DateEdn { get; set; }
        public int? PublisherUserId { get; set; }

        public virtual User? PublisherUser { get; set; }
    }
}
