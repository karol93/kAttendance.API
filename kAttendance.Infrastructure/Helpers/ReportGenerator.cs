using kAttendance.Infrastructure.Helpers.Excel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace kAttendance.Infrastructure.Helpers
{
   public static class ReportGenerator
   {
      private const int DAYS_COLUMN_START = 4;
      private const int DAYS_ROW = 3;

      private const int PEOPLE_ROW_START = 5;

      private const int SUM_PER_PERSON_COLUMN = 35;

      public static byte[] GetReport(List<ReportDataBuilder> reportsDataBuilders)
      {
         using (IExcelBuilder excelBuilder = new ExcelBuilder($"{DateTime.Now:yyyy-MM-dd}.xlsx"))
         {
            reportsDataBuilders.ForEach(p => Build(excelBuilder, p));
            return excelBuilder.GetExcelAsByte();
         }
      }

      private static void Build(IExcelBuilder excelBuilder, ReportDataBuilder reportDataBuilder)
      {
         var sheetId = excelBuilder.CreateSheet(reportDataBuilder.ReportName);

         BuildTitle(excelBuilder, sheetId, reportDataBuilder);
         BuildSubTitle(excelBuilder, sheetId, reportDataBuilder);
         BuildDays(excelBuilder, sheetId, reportDataBuilder);
         BuildPeopleAttendancesList(excelBuilder, sheetId, reportDataBuilder);
         BuildSumPerDayRow(excelBuilder, sheetId, reportDataBuilder);
         BuildSumPerPerson(excelBuilder, sheetId, reportDataBuilder);
         excelBuilder.AutoFitColumns(sheetId,3.0);
      }

      private static void BuildTitle(IExcelBuilder excelBuilder, Guid sheetId, ReportDataBuilder reportDataBuilder)
      {
         excelBuilder.AddValueToCell(sheetId, new ExcelCell(1, 1), reportDataBuilder.Title);
         excelBuilder.MergeCells(sheetId, new ExcelCell(1, 1), new ExcelCell(1, 35));
         excelBuilder.CenterInHorizontal(sheetId, new ExcelCell(1, 1), new ExcelCell(1, 35));
      }

      private static void BuildSubTitle(IExcelBuilder excelBuilder, Guid sheetId, ReportDataBuilder reportDataBuilder)
      {
         excelBuilder.AddValueToCell(sheetId, new ExcelCell(2, 1), reportDataBuilder.SubTitle);
         excelBuilder.MergeCells(sheetId, new ExcelCell(2, 1), new ExcelCell(2, 35));
         excelBuilder.CenterInHorizontal(sheetId, new ExcelCell(2, 1), new ExcelCell(2, 35));
      }

      private static void BuildDays(IExcelBuilder excelBuilder, Guid sheetId, ReportDataBuilder reportDataBuilder)
      {
         int columnStart = DAYS_COLUMN_START;
         foreach (var day in reportDataBuilder.Days)
         {
            excelBuilder.AddValueToCell(sheetId, new ExcelCell(DAYS_ROW, columnStart), day);
            columnStart++;
         }
      }

      private static void BuildPeopleAttendancesList(IExcelBuilder excelBuilder, Guid sheetId, ReportDataBuilder reportDataBuilder)
      {
         int orderNo = 1;
         int rowStart = PEOPLE_ROW_START;
         foreach (var person in reportDataBuilder.People)
         {
            excelBuilder.AddValueToCell(sheetId, new ExcelCell(rowStart, 1), orderNo);
            excelBuilder.AddValueToCell(sheetId, new ExcelCell(rowStart, 2), person.FullName);
            excelBuilder.AddValueToCell(sheetId, new ExcelCell(rowStart, 3), person.Year, Color.Tomato);

            int columnStart = DAYS_COLUMN_START;
            foreach (var attendance in person.Attendances)
            {
               excelBuilder.AddValueToCell(sheetId, new ExcelCell(rowStart, columnStart), attendance ? 1 : 0);
               columnStart++;
            }

            orderNo++;
            rowStart++;
         }
      }

      private static void BuildSumPerDayRow(IExcelBuilder excelBuilder, Guid sheetId, ReportDataBuilder reportDataBuilder)
      {
         int rowWithSum = reportDataBuilder.People.Count() + PEOPLE_ROW_START;
         int columnStart = DAYS_COLUMN_START;
         foreach (var day in reportDataBuilder.Days)
         {
            excelBuilder.SumCells(sheetId, new ExcelCell(rowWithSum, columnStart), new ExcelCell(PEOPLE_ROW_START, columnStart), new ExcelCell(rowWithSum - 1, columnStart));
            excelBuilder.SetCellBackground(sheetId, new ExcelCell(rowWithSum, columnStart), Color.Gold);
            columnStart++;
         }
      }

      private static void BuildSumPerPerson(IExcelBuilder excelBuilder, Guid sheetId,
         ReportDataBuilder reportDataBuilder)
      {
         excelBuilder.AddValueToCell(sheetId, new ExcelCell(DAYS_ROW + 1, SUM_PER_PERSON_COLUMN), "SUMA", Color.LawnGreen);
         int rowStart = PEOPLE_ROW_START;
         foreach (var person in reportDataBuilder.People)
         {
            excelBuilder.SumCells(sheetId, new ExcelCell(rowStart, SUM_PER_PERSON_COLUMN), new ExcelCell(rowStart, DAYS_COLUMN_START), new ExcelCell(rowStart, SUM_PER_PERSON_COLUMN - 1));
            excelBuilder.SetCellBackground(sheetId, new ExcelCell(rowStart, SUM_PER_PERSON_COLUMN), Color.LawnGreen);
            rowStart++;
         }
      }
   }
}
