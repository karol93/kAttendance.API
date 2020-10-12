using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace kAttendance.Infrastructure.Helpers.Excel
{
   public class ExcelBuilder : IExcelBuilder
   {
      private readonly ExcelPackage _excelPackage;
      private readonly IDictionary<Guid, ExcelWorksheet> _sheets;

      public ExcelBuilder()
      {
         _excelPackage = new ExcelPackage();
         _sheets = new Dictionary<Guid, ExcelWorksheet>();
      }

      public ExcelBuilder(string fileName)
      {
         _excelPackage = new ExcelPackage(new FileInfo(fileName));
         _sheets = new Dictionary<Guid, ExcelWorksheet>();
      }

      public Guid CreateSheet(string sheetName)
      {
         var sheet = _excelPackage.Workbook.Worksheets.Add(sheetName);
         var sheetId = Guid.NewGuid();
         _sheets.Add(sheetId, sheet);
         return sheetId;
      }

      public byte[] GetExcelAsByte()
      {
         return _excelPackage.GetAsByteArray();
      }

      public Stream GetExcelAsStream()
      {
         return new MemoryStream(_excelPackage.GetAsByteArray());
      }

      public void AddValueToCell(Guid sheetId, ExcelCell cell, object value)
      {
         var sheet = GetSheet(sheetId);
         sheet.Cells[cell.Row, cell.Column].Value = value;
      }

      public void AddValueToCell(Guid sheetId, ExcelCell cell, object value, Color color)
      {
         var sheet = GetSheet(sheetId);
         sheet.Cells[cell.Row, cell.Column].Value = value;
         SetCellBackground(sheetId, cell, color);
      }

      public void MergeCells(Guid sheetId, ExcelCell cellFrom, ExcelCell cellTo)
      {
         var sheet = GetSheet(sheetId);
         sheet.Cells[cellFrom.Row, cellFrom.Column, cellTo.Row, cellTo.Column].Merge = true;
      }

      public void CenterInHorizontal(Guid sheetId, ExcelCell cellFrom, ExcelCell cellTo)
      {
         var sheet = GetSheet(sheetId);
         sheet.Cells[cellFrom.Row, cellFrom.Column, cellTo.Row, cellTo.Column].Style.HorizontalAlignment =
            ExcelHorizontalAlignment.Center;
      }

      public void SetCellBackground(Guid sheetId, ExcelCell cell, Color color)
      {
         var sheet = GetSheet(sheetId);
         sheet.Cells[cell.Row, cell.Column].Style.Fill.PatternType = ExcelFillStyle.Solid;
         sheet.Cells[cell.Row, cell.Column].Style.Fill.BackgroundColor.SetColor(color);
      }

      public void SumCells(Guid sheetId, ExcelCell cell, ExcelCell sumFromCell, ExcelCell sumToCell)
      {
         var sheet = GetSheet(sheetId);
         var fromCellAddress = sheet.Cells[sumFromCell.Row, sumFromCell.Column].Address;
         var toCellAddress = sheet.Cells[sumToCell.Row, sumToCell.Column].Address;
         sheet.Cells[cell.Row, cell.Column].Formula =
            $"SUM({fromCellAddress}:{toCellAddress})";
      }

      public void AutoFitColumns(Guid sheetId, double minWidth)
      {
         var sheet = GetSheet(sheetId);
         sheet.Cells[sheet.Dimension.Address].AutoFitColumns(minWidth);
      }

      private ExcelWorksheet GetSheet(Guid id)
      {
         if (!_sheets.ContainsKey(id))
            throw new ArgumentException($"Arkusz o podanym identyfikatorze {id} nie istnieje.");
         return _sheets[id];
      }

      public void Dispose()
      {
         _excelPackage?.Dispose();
      }
   }
}

