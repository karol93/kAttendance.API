namespace kAttendance.Infrastructure.Helpers.Excel
{
    public class ExcelCell
    {
       public ExcelCell(int row,int column)
       {
          Row = row;
          Column = column;
       }
       public int Row { get; }
       public int Column { get; }
    }
}
