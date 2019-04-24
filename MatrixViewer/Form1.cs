using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZachLib;
using ZachLib.Statistics;

namespace MatrixViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void grdData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.Timer.Stop();
            Debug.WriteLine(
                "Loading Form: {0} - {1}", 
                Program.Timer.ElapsedMilliseconds, 
                Program.Timer.ElapsedTicks
            );
            /*DataTable table = new DataTable();
                table.BeginLoadData();
                table.Load(new MatrixDataReader(Matrix.Deserialize(path)));
                table.EndLoadData();
                grdData.DataSource = bindingSource1;
                bindingSource1.DataSource = table;*/
        }

        private int columnWidth = 0;
        private int labelWidthHeight = 0;
        private int numCols = 0;
        public void OpenFile(string path)
        {
            var elementFont = new Font("Century Schoolbook", 8.25f, FontStyle.Regular);
            var elementBackColor = lstView.BackColor;
            var elementForeColor = lstView.ForeColor;
            var labelsFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            var labelsBackColor = Color.LightGreen;
            var labelsForeColor = Color.Black;
            //elementBackColorBrush = new SolidBrush(elementBackColor);
            //foreColorBrush = new SolidBrush(elementForeColor);

            /*Graphics gfxText = Graphics.FromImage(new Bitmap(64, 64));
            var font = new Font("Microsoft Sans Serif", 8.25f);
            float digit = gfxText.MeasureString("0", elementFont).Width;
            float digits = gfxText.MeasureString("1234567890", elementFont).Width;*/

            Program.Timer.Restart();

            /*lstView.OwnerDraw = true;
            lstView.DrawColumnHeader += LstView_DrawColumnHeader;
            lstView.DrawItem += LstView_DrawItem;
            lstView.DrawSubItem += LstView_DrawSubItem;*/

            lstView.BeginUpdate();
            lstView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lstView.Font = labelsFont;
            //lstView.BackColor = labelsBackColor;
            var matrix = Matrix.Deserialize(path, out Matrix.MatrixFileHeader header);

            columnWidth = (
                header.MaxDigits + 
                (header.HasNegatives ? 1 : 0) + 
                (header.NumDecimalPlaces > 0 ? header.NumDecimalPlaces + 1 : 0)) * 10;
            labelWidthHeight = header.NumRows.GetDigitsCount() * 10;
            numCols = header.NumCols;
            /*lstView.SmallImageList = new ImageList();
            lstView.SmallImageList.ImageSize = new Size(columnWidth, labelWidthHeight);*/
            /*var bitmap = new Bitmap(columnWidth, labelWidthHeight);
            var gfxTemp = Graphics.FromImage(bitmap);
            gfxTemp.FillRectangle()*/
            //lstView.SmallImageList.Images.Add(new Bitmap(columnWidth, labelWidthHeight));

            //lstView.Columns.Add("colCorner", "X", labelWidthHeight, HorizontalAlignment.Center, 0);
            lstView.Columns.Add("", labelWidthHeight, HorizontalAlignment.Center);
            for (int j = 0; j < matrix.NumCols; ++j)
            {
                lstView.Columns.Add((j + 1).ToString(), columnWidth, HorizontalAlignment.Center);
            }

            Program.Timer.Stop();
            Debug.WriteLine(
                "Setup: {0} - {1}",
                Program.Timer.ElapsedMilliseconds,
                Program.Timer.ElapsedTicks
            );
            Program.Timer.Restart();

            int rowStart = 0;
            for (int i = 0; i < matrix.NumRows; ++i)
            {
                var row = new ListViewItem()
                {
                    UseItemStyleForSubItems = false
                };
                row.SubItems[0] = new ListViewItem.ListViewSubItem(
                    row, (i + 1).ToString(), 
                    labelsForeColor, 
                    elementBackColor, 
                    labelsFont
                );
                for (int j = 0; j < matrix.NumCols; ++j)
                {
                    row.SubItems.Add(
                        matrix._compactedMatrix[rowStart + j].ToString(), 
                        elementForeColor, 
                        elementBackColor,
                        elementFont
                    );
                }
                lstView.Items.Add(row);
                rowStart += matrix.NumCols;
            }

            Program.Timer.Stop();
            Debug.WriteLine(
                "Filling ListView: {0} - {1}",
                Program.Timer.ElapsedMilliseconds,
                Program.Timer.ElapsedTicks
            );
            Program.Timer.Restart();
            Width = labelWidthHeight +
                (columnWidth * header.NumCols) +
                (3 * (header.NumCols + 2));

            lstView.EndUpdate();
            lstView.Update();

            Program.Timer.Stop();
            Debug.WriteLine(
                "Update Form: {0} - {1}",
                Program.Timer.ElapsedMilliseconds,
                Program.Timer.ElapsedTicks
            );
        }

        /*private int itemHeightOffset = 0;
        private List<Rectangle> columnBounds = new List<Rectangle>();
        private void LstView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            columnBounds.Add(e.Bounds);
            var newBounds = e.Bounds;
            newBounds.Height = labelWidthHeight;
            if (e.ColumnIndex == 0)
            {
                e.Graphics.FillRectangle(Brushes.White, newBounds);
                itemHeightOffset = labelWidthHeight - e.Bounds.Height;
            }
            else
                e.Graphics.FillRectangle(Brushes.LightGreen, newBounds);
            e.DrawText(TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter);
        }

        private int itemYPos = 0;
        private void LstView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;

            if (e.ItemIndex == 0)
                Width = e.Bounds.Width;
            itemYPos = e.Bounds.Y + itemHeightOffset;
            var newBounds = e.Bounds;
            newBounds.Y = itemYPos;
            //e.Graphics.FillRectangle(Brushes.LightGreen, newBounds);
            //e.DrawText(TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }

        private SolidBrush elementBackColorBrush = null;
        private SolidBrush foreColorBrush = null;
        private readonly StringFormat labelFormat = new StringFormat() {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Center
        };
        private readonly StringFormat elementFormat = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        private void LstView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            var newBounds = e.Bounds;
            //newBounds.Y = itemYPos;
            if (e.ColumnIndex == 0)
            {
                e.Graphics.FillRectangle(Brushes.LightGreen, newBounds);
                e.Graphics.DrawString(
                    e.SubItem.Text,
                    e.SubItem.Font,
                    foreColorBrush,
                    newBounds,
                    labelFormat
                );
            }
            else
            {
                e.Graphics.FillRectangle(elementBackColorBrush, newBounds);
                e.Graphics.DrawString(
                    e.SubItem.Text,
                    e.SubItem.Font,
                    foreColorBrush,
                    newBounds,
                    elementFormat
                );
            }
        }*/
    }
}
