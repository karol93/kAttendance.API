using System;
using System.Drawing;
using System.IO;

namespace kAttendance.Infrastructure.Helpers.Excel
{
   public interface IExcelBuilder : IDisposable
   {
      Guid CreateSheet(string sheetName);
      byte[] GetExcelAsByte();
      Stream GetExcelAsStream();
      void AddValueToCell(Guid sheetId, ExcelCell cell, object value);
      void AddValueToCell(Guid sheetId, ExcelCell cell, object value, Color color);
      void MergeCells(Guid sheetId, ExcelCell cellFrom, ExcelCell cellTo);
      void CenterInHorizontal(Guid sheetId, ExcelCell cellFrom, ExcelCell cellTo);
      void SetCellBackground(Guid sheetId, ExcelCell cell, Color color);
      void SumCells(Guid sheetId, ExcelCell cell, ExcelCell sumFromCell, ExcelCell sumToCell);
      void AutoFitColumns(Guid sheetId, double minWidth);
   }
}
