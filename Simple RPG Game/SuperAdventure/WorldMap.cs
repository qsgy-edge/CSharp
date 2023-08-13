using Engine;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SuperAdventure
{
    public partial class WorldMap : Form
    {
        private readonly Assembly thisAssembly = Assembly.GetExecutingAssembly();
        private readonly Bitmap fogImage;
        private readonly Player player;

        public WorldMap(Player player)
        {
            InitializeComponent();

            this.player = player;
            fogImage = FilenameToBitmap("FogLocation");

            DisplayImage(pic_0_2, 5, "HerbalistsGarden");
            DisplayImage(pic_1_2, 4, "HerbalistsHut");
            DisplayImage(pic_2_0, 7, "FarmFields");
            DisplayImage(pic_2_1, 6, "Farmhouse");
            DisplayImage(pic_2_2, 2, "TownSquare");
            DisplayImage(pic_2_3, 3, "TownGate");
            DisplayImage(pic_2_4, 8, "Bridge");
            DisplayImage(pic_2_5, 9, "SpiderForest");
            DisplayImage(pic_3_2, 1, "Home");
        }

        private Bitmap FilenameToBitmap(string imageFileName)
        {
            string fullFileName =
                $"{thisAssembly.GetName().Name}.Images.{imageFileName}.png";

            using (Stream resourceStream =
                thisAssembly.GetManifestResourceStream(fullFileName))
            {
                if (resourceStream != null)
                {
                    return new Bitmap(resourceStream);
                }
            }

            return null;
        }

        private void DisplayImage(PictureBox pictureBox, int locationID, string imageName)
        {
            if (player.LocationsVisited.Contains(locationID))
            {
                pictureBox.Image = FilenameToBitmap(imageName);
                // 用红框标记玩家当前位置
                if (player.CurrentLocation.ID == locationID)
                {
                    using (Graphics g = Graphics.FromImage(pictureBox.Image))
                    {
                        int borderWidth = 3;

                        g.DrawRectangle(new Pen(Brushes.Red, borderWidth),
                            new Rectangle(0, 0, pictureBox.Image.Width - borderWidth, pictureBox.Image.Height - borderWidth));
                    }
                }
            }
            else
            {
                pictureBox.Image = fogImage;
            }
        }
    }
}