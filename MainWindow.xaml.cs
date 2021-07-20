using System.Windows;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System.IO;
using Microsoft.Win32;

namespace SfDataGridDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            // Based on the column mapping name and the cell type, we can change the cell values while exporting to excel.
            if (e.CellType == ExportCellType.RecordCell && e.ColumnName == "IsShipped")
            {
                //add the checkbox into excel shhet
                var checkbox = e.Range.Worksheet.CheckBoxes.AddCheckBox(e.Range.Row, e.Range.Column, 20, 20);

                //set the checked or unchecked state based on cell value
                if (e.CellValue.ToString().ToLower() == "true")
                    checkbox.CheckState = ExcelCheckState.Checked;
                else if (e.CellValue.ToString().ToLower() == "false")
                    checkbox.CheckState = ExcelCheckState.Unchecked;

                //Created check box with cell link
                checkbox.LinkedCell = e.Range.Worksheet[e.Range.AddressLocal];

                e.Handled = true;
            }
        }

        private void btnExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            options.CellsExportingEventHandler = CellExportingHandler;
            var excelEngine = sfDataGrid.ExportToExcel(sfDataGrid.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            SaveFileDialog sfd = new SaveFileDialog
            {
                FilterIndex = 2,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
            };

            if (sfd.ShowDialog() == true)
            {
                using (Stream stream = sfd.OpenFile())
                {

                    if (sfd.FilterIndex == 1)
                        workBook.Version = ExcelVersion.Excel97to2003;

                    else if (sfd.FilterIndex == 2)
                        workBook.Version = ExcelVersion.Excel2010;

                    else
                        workBook.Version = ExcelVersion.Excel2013;
                    workBook.SaveAs(stream);
                }

                //Message box confirmation to view the created workbook.
                if (MessageBox.Show("Do you want to view the workbook?", "Workbook has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {

                    //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }
    }
}
         
   

