using System.Drawing;
using FluentAssertions;
using kAttendance.Infrastructure.Helpers.Excel;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using Xunit;

namespace kAttendance.IntegrationTests
{
   public class ExcelBuilderTests
   {
      private Stream _excelStream;
      private readonly string _sheetName = "test sheet";

      public ExcelBuilderTests()
      {

      }

      [Fact]
      public void ShouldCreateSheet()
      {
         using (ExcelBuilder excelBuilder = new ExcelBuilder())
         {
            var sheetId = excelBuilder.CreateSheet(_sheetName);
            _excelStream = excelBuilder.GetExcelAsStream();
         }

         using (ExcelPackage excelPackage = new ExcelPackage(_excelStream))
         {
            var sheet = excelPackage.Workbook.Worksheets[_sheetName];
            sheet.Should().NotBeNull();
            sheet.Name.Should().Be(_sheetName);
         }
      }

      [Fact]
      public void ShouldWriteValueToCell()
      {
         string expectedValue = "test";

         using (ExcelBuilder excelBuilder = new ExcelBuilder())
         {
            var sheetId = excelBuilder.CreateSheet(_sheetName);
            excelBuilder.AddValueToCell(sheetId, new ExcelCell(1, 1), expectedValue);
            _excelStream = excelBuilder.GetExcelAsStream();
         }

         using (ExcelPackage excelPackage = new ExcelPackage(_excelStream))
         {
            var sheet = excelPackage.Workbook.Worksheets[_sheetName];
            var value = sheet.Cells[1, 1].Value;
            value.Should().Be(expectedValue);
         }
      }

      [Fact]
      public void ShouldMergeTwoCells()
      {
         using (ExcelBuilder excelBuilder = new ExcelBuilder())
         {
            var sheetId = excelBuilder.CreateSheet(_sheetName);
            excelBuilder.MergeCells(sheetId,new ExcelCell(1,1),new ExcelCell(1,2));
            _excelStream = excelBuilder.GetExcelAsStream();
         }

         using (ExcelPackage excelPackage = new ExcelPackage(_excelStream))
         {
            var sheet = excelPackage.Workbook.Worksheets[_sheetName];
            var isFirstCellMerged = sheet.Cells[1, 1].Merge;
            var isSecondCellMerged = sheet.Cells[1, 2].Merge;
            var isThirdCellMerged = sheet.Cells[1, 3].Merge;
            isFirstCellMerged.Should().Be(true);
            isSecondCellMerged.Should().Be(true);
            isThirdCellMerged.Should().Be(false);
         }
      }

      [Fact]
      public void ShouldCenterContentInMergedTwoCells()
      {
         using (ExcelBuilder excelBuilder = new ExcelBuilder())
         {
            var sheetId = excelBuilder.CreateSheet(_sheetName);
            excelBuilder.MergeCells(sheetId, new ExcelCell(1, 1), new ExcelCell(1, 2));
            excelBuilder.CenterInHorizontal(sheetId, new ExcelCell(1, 1), new ExcelCell(1, 2));
            _excelStream = excelBuilder.GetExcelAsStream();
         }

         using (ExcelPackage excelPackage = new ExcelPackage(_excelStream))
         {
            var sheet = excelPackage.Workbook.Worksheets[_sheetName];
            var firstMergedCell = sheet.Cells[1, 1].Style.HorizontalAlignment;
            var secondMergedCell = sheet.Cells[1, 2].Style.HorizontalAlignment;
            firstMergedCell.Should().Be(ExcelHorizontalAlignment.Center);
            secondMergedCell.Should().Be(ExcelHorizontalAlignment.Center);
         }
      }

      [Fact]
      public void ShouldSetBackgroundColorInCell()
      {
         Color expectedColor = Color.Green;
         using (ExcelBuilder excelBuilder = new ExcelBuilder())
         {
            var sheetId = excelBuilder.CreateSheet(_sheetName);
            excelBuilder.SetCellBackground(sheetId, new ExcelCell(1, 1), expectedColor);
            _excelStream = excelBuilder.GetExcelAsStream();
         }

         using (ExcelPackage excelPackage = new ExcelPackage(_excelStream))
         {
            var sheet = excelPackage.Workbook.Worksheets[_sheetName];
            var color = sheet.Cells[1, 1].Style.Fill.BackgroundColor;
            color.Rgb.Should().Be(string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", expectedColor.A, expectedColor.R, expectedColor.G, expectedColor.B));
         }
      }

      [Theory]
      [InlineData(1, 1, 1, 2, 1, 3, 1, 4)]
      [InlineData(1, 1, 2, 1, 3, 1, 4, 1)]
      [InlineData(1, 1, 1, 2, 2, 1, 2, 2)]
      public void ShouldSumRangeOfCells(int x, int y, int x1, int y1, int x2, int y2, int x3, int y3)
      {
         int a = 3;
         int b = 5;
         int c = 8;
         int d = 12;
         int expectedSum = a + b + c + d;

         using (ExcelBuilder excelBuilder = new ExcelBuilder())
         {
            var sheetId = excelBuilder.CreateSheet(_sheetName);
            excelBuilder.AddValueToCell(sheetId, new ExcelCell(x, y), a);
            excelBuilder.AddValueToCell(sheetId, new ExcelCell(x1, y1), b);
            excelBuilder.AddValueToCell(sheetId, new ExcelCell(x2, y2), c);
            excelBuilder.AddValueToCell(sheetId, new ExcelCell(x3, y3), d);
            excelBuilder.SumCells(sheetId, new ExcelCell(5, 5), new ExcelCell(x, y), new ExcelCell(x3, y3));
            _excelStream = excelBuilder.GetExcelAsStream();
         }

         using (ExcelPackage excelPackage = new ExcelPackage(_excelStream))
         {
            var sheet = excelPackage.Workbook.Worksheets[_sheetName];
            sheet.Cells[5, 5].Calculate();
            var value = sheet.Cells[5, 5].Value;
            value.Should().Be(expectedSum);
         }
      }
   }
}
