using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefiKindom_QuestRunner.Abis.Objects
{
    internal class DfkProfile
    {
        public DfkProfile()
        {

        }

        public DfkProfile(int _id, string _owner, string _name, DateTime _created, int _picId, int _heroId, int _points)
        {
            this.Id = _id;
            this.Address = _owner;
            this.Name = _name;
            this.Created = _created;
            this.PictureId = _picId;
            this.HeroId = _heroId;
            this.Points = _points;
        }

        public int Id { get; set; }

        public string Address { get; set; }

        public string Name { get; set; }

        public DateTime Created { get; set; }

        public int PictureId { get; set; }

        public int HeroId { get; set; }

        public int Points { get; set; }
    }
}
