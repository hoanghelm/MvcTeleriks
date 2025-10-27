namespace WIRS.Services.Models
{
    public class MenuModel
    {
        public double MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string MenuUrl { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool HasChildren { get; set; }
        public List<MenuModel> Children { get; set; } = new List<MenuModel>();
    }
}