using System.Collections.ObjectModel;

namespace WIRS.DataAccess.Entities
{
    public class Menu
    {
        public Menu(decimal id, string text, string url, bool IsEnable)
        {
            _id = id;
            _text = text;
            _url = url;
            _IsEnable = IsEnable;
        }

        private decimal _id;

        public decimal Id
        {
            get => _id;
            set => _id = value;
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => _text = value;
        }
        private string _url;

        public string URL
        {
            get => _url;
            set => _url = value;
        }

        private bool _IsEnable;
        public bool IsEnable
        {
            get => _IsEnable;
            set => _IsEnable = value;
        }
        private Collection<Menu> _subMenus;
        public Collection<Menu> SubMenus
        {
            get
            {
                if (this._subMenus == null)
                {
                    this._subMenus = new Collection<Menu>();
                }

                return this._subMenus;
            }
        }
    }
}