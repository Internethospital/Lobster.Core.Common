using Microsoft.AspNetCore.Http;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Common.Helper
{
    /// <summary>
    /// 导出Excel
    /// </summary>
    public class ExcelHelper
    {
        /// <summary>  
        /// DataTable导出到Excel文件  
        /// </summary>  
        /// <param name="dtSource">源DataTable</param>  
        /// <param name="strHeaderText">表头文本</param>  
        /// <param name="strFileName">保存位置</param>  
        /// <Author></Author>  
        public static void Export(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = Export(dtSource, strHeaderText, new Dictionary<string, string>(), new Dictionary<string, string>()))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        /// <summary>
        /// DataTable导出到Excel文件，可以自定义导出那些列
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="strHeaderText"></param>
        /// <param name="columnNames"></param>
        /// <param name="strFileName"></param>
        public static void Export(DataTable dtSource, string strHeaderText, Dictionary<string, string> columnNames, string strFileName)
        {
            using (MemoryStream ms = Export(dtSource, strHeaderText, columnNames, new Dictionary<string, string>()))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        /// <summary>
        ///  DataTable导出到Excel文件，可以自定义导出列和设置列的数据格式
        /// </summary>
        /// <param name="dtSource"></param>
        /// <param name="strHeaderText"></param>
        /// <param name="columnNames"></param>
        /// <param name="dataformats"></param>
        /// <param name="strFileName"></param>
        public static void Export(DataTable dtSource, string strHeaderText, Dictionary<string, string> columnNames, Dictionary<string, string> dataformats, string strFileName)
        {
            using (MemoryStream ms = Export(dtSource, strHeaderText, columnNames, dataformats))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }


        /// <summary>  
        /// DataTable导出到Excel的MemoryStream  
        /// </summary>  
        /// <param name="dtSource">源DataTable</param>  
        /// <param name="strHeaderText">表头文本</param>  
        /// <Author> 2010-5-8 22:21:41</Author>  
        public static MemoryStream Export(DataTable dtSource, string strHeaderText, Dictionary<string, string> columnNames, Dictionary<string, string> dataformats)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "kakake"; //填加xls文件作者信息  
                si.ApplicationName = ""; //填加xls文件创建程序信息  
                si.LastAuthor = ""; //填加xls文件最后保存者信息  
                si.Comments = "说明信息"; //填加xls文件作者信息  
                si.Title = ""; //填加xls文件标题信息  
                si.Subject = "";//填加文件主题信息  
                si.CreateDateTime = DateTime.Now.ToCstTime();
                workbook.SummaryInformation = si;
            }
            #endregion

            HSSFCellStyle dateStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFDataFormat format = (HSSFDataFormat)workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            HSSFCellStyle customStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFDataFormat customformat = (HSSFDataFormat)workbook.CreateDataFormat();

            //取得列宽  
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }



            int rowIndex = 0;
            int index = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = (HSSFSheet)workbook.CreateSheet();
                    }

                    #region 表头及样式
                    {
                        HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        HSSFFont font = (HSSFFont)workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        headerRow.GetCell(0).CellStyle = headStyle;
                        if (columnNames.Count > 0)
                        {
                            sheet.AddMergedRegion(new Region(0, 0, 0, columnNames.Count - 1));
                        }
                        else
                            sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                        //headerRow.Dispose();
                    }
                    #endregion


                    #region 列头及样式
                    {
                        HSSFRow headerRow = (HSSFRow)sheet.CreateRow(1);


                        HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        HSSFFont font = (HSSFFont)workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);

                        index = 0;
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            if (columnNames.Count > 0)
                            {
                                if (columnNames.ContainsKey(column.ColumnName))
                                {

                                    headerRow.CreateCell(index).SetCellValue(columnNames[column.ColumnName]);
                                    headerRow.GetCell(index).CellStyle = headStyle;

                                    //设置列宽  
                                    sheet.SetColumnWidth(index, (Encoding.GetEncoding(936).GetBytes(columnNames[column.ColumnName]).Length + 1) * 256);

                                    index++;
                                }
                            }
                            else
                            {
                                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                                headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                                //设置列宽  
                                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                            }
                        }
                        //headerRow.Dispose();
                    }
                    #endregion

                    rowIndex = 2;
                }
                #endregion


                #region 填充内容
                index = 0;
                HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    if (columnNames.Count > 0)
                    {
                        if (columnNames.ContainsKey(column.ColumnName))
                        {
                            HSSFCell newCell = (HSSFCell)dataRow.CreateCell(index);

                            string drValue = row[column].ToString();

                            switch (column.DataType.ToString())
                            {
                                case "System.String"://字符串类型  
                                    newCell.SetCellValue(drValue);
                                    break;
                                case "System.DateTime"://日期类型  
                                    DateTime dateV;
                                    DateTime.TryParse(drValue, out dateV);
                                    newCell.SetCellValue(dateV);

                                    newCell.CellStyle = dateStyle;//格式化显示  
                                    break;
                                case "System.Boolean"://布尔型  
                                    bool boolV = false;
                                    bool.TryParse(drValue, out boolV);
                                    newCell.SetCellValue(boolV);
                                    break;
                                case "System.Int16"://整型  
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Byte":
                                    int intV = 0;
                                    int.TryParse(drValue, out intV);
                                    newCell.SetCellValue(intV);
                                    break;
                                case "System.Decimal"://浮点型  
                                case "System.Double":
                                    double doubV = 0;
                                    double.TryParse(drValue, out doubV);
                                    newCell.SetCellValue(doubV);
                                    //HSSFCellStyle celldoubleStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                                    //int pos = Convert.ToString(doubV).Length - Convert.ToString(doubV).IndexOf('.') - 1;
                                    //if (pos == 4)
                                    //{
                                    //    celldoubleStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.0000");
                                    //}
                                    //else if (pos == 2)
                                    //{
                                    //    celldoubleStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                    //}
                                    //newCell.CellStyle = celldoubleStyle;
                                    break;
                                case "System.DBNull"://空值处理  
                                    newCell.SetCellValue("");
                                    break;
                                default:
                                    newCell.SetCellValue("");
                                    break;
                            }

                            if (dataformats.ContainsKey(column.ColumnName))
                            {
                                customStyle.DataFormat = customformat.GetFormat(dataformats[column.ColumnName]);
                                newCell.CellStyle = customStyle;
                            }

                            index++;
                        }
                    }
                    else
                    {
                        HSSFCell newCell = (HSSFCell)dataRow.CreateCell(column.Ordinal);

                        string drValue = row[column].ToString();

                        switch (column.DataType.ToString())
                        {
                            case "System.String"://字符串类型  
                                newCell.SetCellValue(drValue);
                                break;
                            case "System.DateTime"://日期类型  
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);

                                newCell.CellStyle = dateStyle;//格式化显示  
                                break;
                            case "System.Boolean"://布尔型  
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                newCell.SetCellValue(boolV);
                                break;
                            case "System.Int16"://整型  
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;
                            case "System.Decimal"://浮点型  
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                newCell.SetCellValue(doubV);
                                //HSSFCellStyle celldoubleStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                                //int pos = Convert.ToString(doubV).Length - Convert.ToString(doubV).IndexOf('.') - 1;
                                //if (pos == 4)
                                //{
                                //    celldoubleStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.0000");
                                //}
                                //else if (pos == 2)
                                //{
                                //    celldoubleStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                                //}
                                //newCell.CellStyle = celldoubleStyle;
                                break;
                            case "System.DBNull"://空值处理  
                                newCell.SetCellValue("");
                                break;
                            default:
                                newCell.SetCellValue("");
                                break;
                        }

                        if (dataformats.ContainsKey(column.ColumnName))
                        {
                            customStyle.DataFormat = customformat.GetFormat(dataformats[column.ColumnName]);
                            newCell.CellStyle = customStyle;
                        }
                    }
                }
                #endregion

                rowIndex++;
            }


            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet.Workbook.Dispose();
                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放 sheet  
                return ms;
            }

        }

        /*
        /// <summary>  
        /// 用于Web导出  
        /// </summary>  
        /// <param name="dtSource">源DataTable</param>  
        /// <param name="strHeaderText">表头文本</param>  
        /// <param name="strFileName">文件名</param>  
        /// <Author> 2010-5-8 22:21:41</Author>  
        public static void ExportByWeb(HttpContext context, DataTable dtSource, string strHeaderText, Dictionary<string, string> columnNames, string strFileName)
        {

            HttpContext curContext = context;

            // 设置编码和附件格式  
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.ContentEncoding = Encoding.UTF8;
            curContext.Response.Charset = "";
            curContext.Response.AppendHeader("Content-Disposition",
                "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));

            curContext.Response.BinaryWrite(Export(dtSource, strHeaderText, columnNames, new Dictionary<string, string>()).GetBuffer());
            //curContext.Response.End();
        }
        */

        /// <summary>读取excel  
        /// 默认第一行为标头  
        /// </summary>  
        /// <param name="strFileName">excel文档路径</param>  
        /// <returns></returns>  
        public static DataTable Import(string strFileName)
        {
            DataTable dt = new DataTable();

            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            HSSFRow headerRow = (HSSFRow)sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                HSSFCell cell = (HSSFCell)headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                HSSFRow row = (HSSFRow)sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }


        #region Excel导入

        /// <summary>
        /// 从Excel取数据并记录到List集合里
        /// </summary>
        /// <param name="cellHeard">单元头的值和名称：{ { "UserName", "姓名" }, { "Age", "年龄" } };</param>
        /// <param name="filePath">保存文件绝对路径</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns>转换后的List对象集合</returns>
        public static List<T> ExcelToEntityList<T>(Dictionary<string, string> cellHeard, string filePath, out StringBuilder errorMsg) where T : new()
        {
            List<T> enlist = new List<T>();
            errorMsg = new StringBuilder();
            try
            {
                return Excel2003ToEntityList<T>(cellHeard, filePath, out errorMsg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 从Excel2003取数据并记录到List集合里
        /// </summary>
        /// <param name="cellHeard">单元头的Key和Value：{ { "UserName", "姓名" }, { "Age", "年龄" } };</param>
        /// <param name="filePath">保存文件绝对路径</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns>转换好的List对象集合</returns>
        private static List<T> Excel2003ToEntityList<T>(Dictionary<string, string> cellHeard, string filePath, out StringBuilder errorMsg) where T : new()
        {
            errorMsg = new StringBuilder(); // 错误信息,Excel转换到实体对象时，会有格式的错误信息
            List<T> enlist = new List<T>(); // 转换后的集合 
            var keys2 = cellHeard.Keys;
            var keys = new List<string>();
            foreach (var item in keys2)
            {
                keys.Add(item);
            }
            try
            {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    IWorkbook workbook = null;
                    ISheet sheet = null;
                    if (Regex.IsMatch(filePath, ".xls$")) // 2003
                    {
                        workbook = new HSSFWorkbook(fs);
                        sheet = (HSSFSheet)workbook.GetSheetAt(0);
                    }
                    else
                    {
                        workbook = new XSSFWorkbook(fs);
                        sheet = (XSSFSheet)workbook.GetSheetAt(0);
                    }

                    for (int i = 2; i <= sheet.LastRowNum; i++) // 从2开始，第0行为单元头
                    {
                        // 1.判断当前行是否空行，若空行就不在进行读取下一行操作，结束Excel读取操作
                        if (sheet.GetRow(i) == null)
                        {
                            break;
                        }
                        T en = new T();
                        string errStr = ""; // 当前行转换时，是否有错误信息，格式为：第1行数据转换异常：XXX列； 
                        for (int j = 0; j < keys.Count; j++)
                        {
                            // 2.若属性头的名称包含'.',就表示是子类里的属性，那么就要遍历子类，eg：UserEn.TrueName
                            if (keys[j].IndexOf(".") >= 0)
                            {
                                // 2.1解析子类属性
                                string[] properotyArray = keys[j].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                string subClassName = properotyArray[0]; // '.'前面的为子类的名称
                                string subClassProperotyName = properotyArray[1]; // '.'后面的为子类的属性名称
                                System.Reflection.PropertyInfo subClassInfo = en.GetType().GetProperty(subClassName); // 获取子类的类型
                                if (subClassInfo != null)
                                {
                                    // 2.1.1 获取子类的实例
                                    var subClassEn = en.GetType().GetProperty(subClassName).GetValue(en, null);
                                    // 2.1.2 根据属性名称获取子类里的属性信息
                                    System.Reflection.PropertyInfo properotyInfo = subClassInfo.PropertyType.GetProperty(subClassProperotyName);
                                    if (properotyInfo != null)
                                    {
                                        try
                                        {
                                            // Excel单元格的值转换为对象属性的值，若类型不对，记录出错信息
                                            properotyInfo.SetValue(subClassEn, GetExcelCellToProperty(properotyInfo.PropertyType, sheet.GetRow(i).GetCell(j)), null);
                                        }
                                        catch (Exception e)
                                        {
                                            if (errStr.Length == 0)
                                            {
                                                errStr = "第" + (i - 1) + "行数据转换异常：";
                                            }
                                            errStr += cellHeard[keys[j]] + "列；";
                                        }

                                    }
                                }
                            }
                            else
                            {
                                // 3.给指定的属性赋值
                                System.Reflection.PropertyInfo properotyInfo = en.GetType().GetProperty(keys[j]);
                                if (properotyInfo != null)
                                {
                                    try
                                    {
                                        // Excel单元格的值转换为对象属性的值，若类型不对，记录出错信息
                                        properotyInfo.SetValue(en, GetExcelCellToProperty(properotyInfo.PropertyType, sheet.GetRow(i).GetCell(j)), null);
                                    }
                                    catch (Exception e)
                                    {
                                        if (errStr.Length == 0)
                                        {
                                            errStr = "第" + (i - 1) + "行数据转换异常：";
                                        }
                                        errStr += cellHeard[keys[j]] + "列；";
                                    }
                                }
                            }
                        }
                        // 若有错误信息，就添加到错误信息里
                        if (errStr.Length > 0)
                        {
                            errorMsg.AppendLine(errStr);
                        }
                        else
                        {
                            enlist.Add(en);
                        }
                    }
                }
                return enlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Excel导入

        #region Excel导出

        /// <summary>
        /// 实体类集合导出到EXCLE2003
        /// </summary>
        /// <param name="cellHeard">单元头的Key和Value：{ { "UserName", "姓名" }, { "Age", "年龄" } };</param>
        /// <param name="enList">数据源</param>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>文件的下载地址</returns>
        public static string EntityListToExcel2003(string webRootPath, Dictionary<string, string> cellHeard, IList enList, string sheetName)
        {
            try
            {
                string fileName = sheetName + "-" + DateTime.Now.ToCstTime().ToString("yyyyMMddHHmmssfff") + ".xls"; // 文件名称
                string urlPath = "/upfiles/excelfiles/" + fileName; // 文件下载的URL地址，供给前台下载 
                var filePath = Path.Combine(webRootPath + @"/upfiles/excelfiles/", fileName);
                LogHelper.Error("导出错误", filePath);
                string directoryName = Path.GetDirectoryName(filePath);
                LogHelper.Error("导出错误", directoryName);
                // 1.检测是否存在文件夹，若不存在就建立个文件夹 
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                // 2.解析单元格头部，设置单元头的中文名称
                HSSFWorkbook workbook = new HSSFWorkbook(); // 工作簿
                ISheet sheet = workbook.CreateSheet(sheetName); // 工作表
                IRow row = sheet.CreateRow(0);
                var keys2 = cellHeard.Keys;
                var keys = new List<string>();
                foreach (var item in keys2)
                {
                    keys.Add(item);
                }
                for (int i = 0; i < keys.Count; i++)
                {
                    row.CreateCell(i).SetCellValue(cellHeard[keys[i]]); // 列名为Key的值
                }

                // 3.List对象的值赋值到Excel的单元格里
                int rowIndex = 1; // 从第二行开始赋值(第一行已设置为单元头)
                foreach (var en in enList)
                {
                    IRow rowTmp = sheet.CreateRow(rowIndex);
                    for (int i = 0; i < keys.Count; i++) // 根据指定的属性名称，获取对象指定属性的值
                    {
                        string cellValue = ""; // 单元格的值
                        object properotyValue = null; // 属性的值
                        System.Reflection.PropertyInfo properotyInfo = null; // 属性的信息

                        // 3.1 若属性头的名称包含'.',就表示是子类里的属性，那么就要遍历子类，eg：UserEn.UserName
                        if (keys[i].IndexOf(".") >= 0)
                        {
                            // 3.1.1 解析子类属性(这里只解析1层子类，多层子类未处理)
                            string[] properotyArray = keys[i].Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                            string subClassName = properotyArray[0]; // '.'前面的为子类的名称
                            string subClassProperotyName = properotyArray[1]; // '.'后面的为子类的属性名称
                            System.Reflection.PropertyInfo subClassInfo = en.GetType().GetProperty(subClassName); // 获取子类的类型
                            if (subClassInfo != null)
                            {
                                // 3.1.2 获取子类的实例
                                var subClassEn = en.GetType().GetProperty(subClassName).GetValue(en, null);
                                // 3.1.3 根据属性名称获取子类里的属性类型
                                properotyInfo = subClassInfo.PropertyType.GetProperty(subClassProperotyName);
                                if (properotyInfo != null)
                                {
                                    properotyValue = properotyInfo.GetValue(subClassEn, null); // 获取子类属性的值
                                }
                            }
                        }
                        else
                        {
                            // 3.2 若不是子类的属性，直接根据属性名称获取对象对应的属性
                            properotyInfo = en.GetType().GetProperty(keys[i]);
                            if (properotyInfo != null)
                            {
                                properotyValue = properotyInfo.GetValue(en, null);
                            }
                        }

                        // 3.3 属性值经过转换赋值给单元格值
                        if (properotyValue != null)
                        {
                            cellValue = properotyValue.ToString();
                            // 3.3.1 对时间初始值赋值为空
                            if (cellValue.Trim() == "0001/1/1 0:00:00" || cellValue.Trim() == "0001/1/1 23:59:59")
                            {
                                cellValue = "";
                            }
                        }
                        cellValue = cellValue == null ? "" : cellValue;
                        // 3.4 填充到Excel的单元格里
                        rowTmp.CreateCell(i).SetCellValue(cellValue);
                    }
                    rowIndex++;
                }

                // 4.生成文件
                FileStream file = new FileStream(filePath, FileMode.Create);
                workbook.Write(file);
                file.Close();

                // 5.返回下载路径
                return urlPath;
            }
            catch (Exception ex)
            {
                LogHelper.Error("导出错误", ex.Message);
                throw ex;
            }
        }

        #endregion Excel导出

        /// <summary>
        /// 保存Excel文件
        /// <para>Excel的导入导出都会在服务器生成一个文件</para>
        /// <para>路径：UpFiles/ExcelFiles</para>
        /// </summary>
        /// <param name="file">传入的文件对象</param>
        /// <returns>如果保存成功则返回文件的位置;如果保存失败则返回空</returns> 
        public static string SaveExcelFile(IFormFile formFile, string webRootPath)
        {
            try
            {
                var fileName = formFile.FileName.Insert(formFile.FileName.LastIndexOf('.'), "-" + DateTime.Now.ToCstTime().ToString("yyyyMMddHHmmssfff"));
                var filePath = Path.Combine(webRootPath + "/upfiles/excelfiles/", fileName);
                string directoryName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                using (var stream = new FileStream(filePath, FileMode.CreateNew))
                {
                    formFile.CopyTo(stream);
                }
                return filePath;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 从Excel获取值传递到对象的属性里
        /// </summary>
        /// <param name="distanceType">目标对象类型</param>
        /// <param name="sourceCell">对象属性的值</param>
        private static Object GetExcelCellToProperty(Type distanceType, ICell sourceCell)
        {
            object rs = distanceType.IsValueType ? Activator.CreateInstance(distanceType) : null;

            // 1.判断传递的单元格是否为空
            if (sourceCell == null || string.IsNullOrEmpty(sourceCell.ToString()))
            {
                return rs;
            }

            // 2.Excel文本和数字单元格转换，在Excel里文本和数字是不能进行转换，所以这里预先存值
            object sourceValue = null;
            switch (sourceCell.CellType)
            {
                case CellType.Blank:
                    break;

                case CellType.Boolean:
                    break;

                case CellType.Error:
                    break;

                case CellType.Formula:
                    break;

                case CellType.Numeric:
                    sourceValue = sourceCell.NumericCellValue;
                    break;

                case CellType.String:
                    sourceValue = sourceCell.StringCellValue;
                    break;

                case CellType.Unknown:
                    break;

                default:
                    break;
            }

            string valueDataType = distanceType.Name;

            // 在这里进行特定类型的处理
            switch (valueDataType.ToLower()) // 以防出错，全部小写
            {
                case "string":
                    rs = sourceValue.ToString();
                    break;
                case "int":
                case "int16":
                case "int32":
                    rs = (int)Convert.ChangeType(sourceCell.NumericCellValue.ToString(), distanceType);
                    break;
                case "float":
                case "single":
                    rs = (float)Convert.ChangeType(sourceCell.NumericCellValue.ToString(), distanceType);
                    break;
                case "datetime":
                    rs = sourceCell.DateCellValue;
                    break;
                case "guid":
                    rs = (Guid)Convert.ChangeType(sourceCell.NumericCellValue.ToString(), distanceType);
                    return rs;
            }
            return rs;
        }
       
        /// <summary>
        /// 应用图片
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="webRootPath"></param>
        /// <returns></returns>
        public static string SaveImage(IFormFile formFile, string webRootPath)
        {
            try
            {
                var fileName = formFile.FileName.Insert(formFile.FileName.LastIndexOf('.'), "-" + DateTime.Now.ToCstTime().ToString("yyyyMMddHHmmssfff"));
                var filePath = Path.Combine(webRootPath + "\\Uploads\\Images\\", fileName);
                var base64Str = string.Empty;
                string directoryName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                if (formFile != null)
                {
                    //文件后缀
                    var fileExtension = Path.GetExtension(formFile.FileName);

                    //判断后缀是否是图片
                    const string fileFilt = ".gif|.jpg|.php|.jsp|.jpeg|.png|......";
                    if (fileExtension == null)
                    {
                        return "上传的文件没有后缀";
                    }
                    if (fileFilt.IndexOf(fileExtension.ToLower(), StringComparison.Ordinal) <= -1)
                    {
                        return "上传的文件不是图片";
                    }

                    //判断文件大小    
                    long length = formFile.Length;
                    //if (length > 1024*1024)
                    //{
                    //    return "上传的文件不能大于1M";
                    //}
                }
                //using (var stream = new FileStream(filePath, FileMode.CreateNew))
                //{
                //    formFile.CopyTo(stream);
                //}
                var filestream = new FileStream(filePath, FileMode.CreateNew);

                byte[] bt = new byte[filestream.Length];

                //调用read读取方法
                filestream.Read(bt, 0, bt.Length);
                base64Str = Convert.ToBase64String(bt);
                filestream.Close();

                //将Base64串写入临时文本文件
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                FileStream fs = new FileStream(filePath, FileMode.Create);
                byte[] data = System.Text.Encoding.Default.GetBytes(base64Str);
                //开始写入
                fs.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
                fs.Flush();
                fs.Close();

                return base64Str;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
