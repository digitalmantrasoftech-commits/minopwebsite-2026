using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Drawing.Printing;
using System.Data;

/// <summary>
/// Summary description for PrintWorkingDurationReport
/// </summary>
public class Monthly_Attendance : DevExpress.XtraReports.UI.XtraReport
{
    private DevExpress.XtraReports.UI.DetailBand Detail;
    private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
    private GroupHeaderBand groupHeaderBand1;
    private PageFooterBand pageFooterBand1;
    private XRLine xrLine5;
    private ReportHeaderBand reportHeaderBand1;
    private XRControlStyle Title;
    private XRControlStyle FieldCaption;
    private XRControlStyle PageInfo;
    private XRControlStyle DataField;
    private XRLabel xrLabel21;
    private XRPageInfo xrPageInfo1;
    private XRPageInfo xrPageInfo2;
    private XRLabel xrLabel19;
    private DevExpress.XtraReports.Parameters.Parameter FromDate;
    private DevExpress.XtraReports.Parameters.Parameter Todate;
    private DevExpress.XtraReports.Parameters.Parameter printby;
    private XRTable xrTable1;
    private XRTableRow xrTableRow1;
    private XRTableCell xrTableCell1;
    private XRTableCell xrTableCell2;
    private XRTableCell xrTableCell3;
    private XRLabel xrLabel26;
    private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
    private GroupHeaderBand GroupHeader1;
    private XRPanel xrPanel1;
    private XRLabel xrLabel5;
    private XRLabel xrLabel6;
    private GroupFooterBand GroupFooter1;
    private XRTable xrTable2;
    private XRTableRow xrTableRow2;
    private XRTableCell xrTableCell4;
    private XRTableCell xrTableCell5;
    private XRTableCell xrTableCell6;
    private GroupHeaderBand GroupHeader2;
    private XRLabel xrLabel3;
    private XRLabel xrLabel4;
    private XRLabel xrLabel8;
    private XRLabel xrLabel9;
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public Monthly_Attendance()
    {
        InitializeComponent();
        //
        // TODO: Add constructor logic here
        //
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            DevExpress.DataAccess.ObjectBinding.ObjectConstructorInfo objectConstructorInfo1 = new DevExpress.DataAccess.ObjectBinding.ObjectConstructorInfo();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.groupHeaderBand1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel26 = new DevExpress.XtraReports.UI.XRLabel();
            this.pageFooterBand1 = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrPageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine5 = new DevExpress.XtraReports.UI.XRLine();
            this.reportHeaderBand1 = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.Title = new DevExpress.XtraReports.UI.XRControlStyle();
            this.FieldCaption = new DevExpress.XtraReports.UI.XRControlStyle();
            this.PageInfo = new DevExpress.XtraReports.UI.XRControlStyle();
            this.DataField = new DevExpress.XtraReports.UI.XRControlStyle();
            this.FromDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.Todate = new DevExpress.XtraReports.Parameters.Parameter();
            this.printby = new DevExpress.XtraReports.Parameters.Parameter();
            this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrPanel1 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.GroupFooter1 = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.GroupHeader2 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.HeightF = 0F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StyleName = "DataField";
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 14.99999F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.SnapLinePadding = new DevExpress.XtraPrinting.PaddingInfo(15, 10, 10, 10, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 15F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // groupHeaderBand1
            // 
            this.groupHeaderBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
            this.groupHeaderBand1.Font = new System.Drawing.Font("Times New Roman", 5F);
            this.groupHeaderBand1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WholePage;
            this.groupHeaderBand1.HeightF = 20F;
            this.groupHeaderBand1.Name = "groupHeaderBand1";
            this.groupHeaderBand1.RepeatEveryPage = true;
            this.groupHeaderBand1.StylePriority.UseFont = false;
            // 
            // xrTable1
            // 
            this.xrTable1.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
            this.xrTable1.SizeF = new System.Drawing.SizeF(1070F, 20F);
            this.xrTable1.StylePriority.UseBorders = false;
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1,
            this.xrTableCell2,
            this.xrTableCell3});
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.StylePriority.UseBorders = false;
            this.xrTableRow1.Weight = 1D;
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.StylePriority.UseBorders = false;
            this.xrTableCell1.Text = "xrTableCell1";
            this.xrTableCell1.Weight = 1D;
            // 
            // xrTableCell2
            // 
            this.xrTableCell2.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell2.Name = "xrTableCell2";
            this.xrTableCell2.StylePriority.UseBorders = false;
            this.xrTableCell2.Text = "xrTableCell2";
            this.xrTableCell2.Weight = 1D;
            // 
            // xrTableCell3
            // 
            this.xrTableCell3.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell3.Name = "xrTableCell3";
            this.xrTableCell3.StylePriority.UseBorders = false;
            this.xrTableCell3.Text = "xrTableCell3";
            this.xrTableCell3.Weight = 1D;
            // 
            // xrLabel26
            // 
            this.xrLabel26.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel26.Font = new System.Drawing.Font("Times New Roman", 8F);
            this.xrLabel26.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel26.Name = "xrLabel26";
            this.xrLabel26.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel26.SizeF = new System.Drawing.SizeF(475.7501F, 14F);
            this.xrLabel26.StylePriority.UseBorders = false;
            this.xrLabel26.StylePriority.UseFont = false;
            this.xrLabel26.Text = "P = Total Present  ,A =Total  Absent ,WO= Total Week Off ,HO=Total  Holiday ";
            // 
            // pageFooterBand1
            // 
            this.pageFooterBand1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel21,
            this.xrPageInfo1,
            this.xrPageInfo2,
            this.xrLabel19,
            this.xrLine5});
            this.pageFooterBand1.HeightF = 17.45834F;
            this.pageFooterBand1.Name = "pageFooterBand1";
            // 
            // xrLabel21
            // 
            this.xrLabel21.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(1F, 3.000014F);
            this.xrLabel21.Name = "xrLabel21";
            this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel21.SizeF = new System.Drawing.SizeF(68F, 14.33333F);
            this.xrLabel21.StylePriority.UseFont = false;
            this.xrLabel21.Text = "Print Date :";
            // 
            // xrPageInfo1
            // 
            this.xrPageInfo1.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(69.49976F, 3.000014F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(124.7498F, 14.33333F);
            this.xrPageInfo1.StyleName = "PageInfo";
            this.xrPageInfo1.StylePriority.UseFont = false;
            this.xrPageInfo1.TextFormatString = "{0:dd-MMM-yy hh:mm tt}";
            // 
            // xrPageInfo2
            // 
            this.xrPageInfo2.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrPageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(510.4154F, 2.333355F);
            this.xrPageInfo2.Name = "xrPageInfo2";
            this.xrPageInfo2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo2.SizeF = new System.Drawing.SizeF(86.29175F, 14F);
            this.xrPageInfo2.StyleName = "PageInfo";
            this.xrPageInfo2.StylePriority.UseFont = false;
            this.xrPageInfo2.StylePriority.UseTextAlignment = false;
            this.xrPageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            this.xrPageInfo2.TextFormatString = "Page {0} of {1}";
            // 
            // xrLabel19
            // 
            this.xrLabel19.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.printby]")});
            this.xrLabel19.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold);
            this.xrLabel19.ForeColor = System.Drawing.Color.Black;
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(740.8307F, 2.000046F);
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel19.SizeF = new System.Drawing.SizeF(328.3331F, 14.99999F);
            this.xrLabel19.StylePriority.UseFont = false;
            this.xrLabel19.StylePriority.UseForeColor = false;
            this.xrLabel19.StylePriority.UseTextAlignment = false;
            this.xrLabel19.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel19.TextFormatString = "Print By : {0}";
            // 
            // xrLine5
            // 
            this.xrLine5.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLine5.BorderWidth = 1F;
            this.xrLine5.LocationFloat = new DevExpress.Utils.PointFloat(1F, 0F);
            this.xrLine5.Name = "xrLine5";
            this.xrLine5.SizeF = new System.Drawing.SizeF(1069F, 2F);
            this.xrLine5.StylePriority.UseBorders = false;
            this.xrLine5.StylePriority.UseBorderWidth = false;
            // 
            // reportHeaderBand1
            // 
            this.reportHeaderBand1.Expanded = false;
            this.reportHeaderBand1.HeightF = 0F;
            this.reportHeaderBand1.Name = "reportHeaderBand1";
            // 
            // Title
            // 
            this.Title.BackColor = System.Drawing.Color.Transparent;
            this.Title.BorderColor = System.Drawing.Color.Black;
            this.Title.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.Title.BorderWidth = 1F;
            this.Title.Font = new System.Drawing.Font("Times New Roman", 21F);
            this.Title.ForeColor = System.Drawing.Color.Black;
            this.Title.Name = "Title";
            // 
            // FieldCaption
            // 
            this.FieldCaption.BackColor = System.Drawing.Color.Transparent;
            this.FieldCaption.BorderColor = System.Drawing.Color.Black;
            this.FieldCaption.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.FieldCaption.BorderWidth = 1F;
            this.FieldCaption.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.FieldCaption.ForeColor = System.Drawing.Color.Black;
            this.FieldCaption.Name = "FieldCaption";
            // 
            // PageInfo
            // 
            this.PageInfo.BackColor = System.Drawing.Color.Transparent;
            this.PageInfo.BorderColor = System.Drawing.Color.Black;
            this.PageInfo.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.PageInfo.BorderWidth = 1F;
            this.PageInfo.Font = new System.Drawing.Font("Arial", 8F);
            this.PageInfo.ForeColor = System.Drawing.Color.Black;
            this.PageInfo.Name = "PageInfo";
            // 
            // DataField
            // 
            this.DataField.BackColor = System.Drawing.Color.Transparent;
            this.DataField.BorderColor = System.Drawing.Color.Black;
            this.DataField.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.DataField.BorderWidth = 1F;
            this.DataField.Font = new System.Drawing.Font("Arial", 9F);
            this.DataField.ForeColor = System.Drawing.Color.Black;
            this.DataField.Name = "DataField";
            this.DataField.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            // 
            // FromDate
            // 
            this.FromDate.Description = "FromDate";
            this.FromDate.Name = "FromDate";
            this.FromDate.Visible = false;
            // 
            // Todate
            // 
            this.Todate.Description = "Todate";
            this.Todate.Name = "Todate";
            this.Todate.ValueInfo = "2018-01-11";
            this.Todate.Visible = false;
            // 
            // printby
            // 
            this.printby.Description = "printby";
            this.printby.Name = "printby";
            this.printby.Visible = false;
            // 
            // GroupHeader1
            // 
            this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel6,
            this.xrLabel5,
            this.xrTable2,
            this.xrPanel1});
            this.GroupHeader1.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("ClassDiv", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.GroupHeader1.HeightF = 89.0834F;
            this.GroupHeader1.Level = 1;
            this.GroupHeader1.Name = "GroupHeader1";
            this.GroupHeader1.PageBreak = DevExpress.XtraReports.UI.PageBreak.BeforeBand;
            // 
            // xrTable2
            // 
            this.xrTable2.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(0.999999F, 24.99999F);
            this.xrTable2.Name = "xrTable2";
            this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2});
            this.xrTable2.SizeF = new System.Drawing.SizeF(1070F, 20F);
            this.xrTable2.StylePriority.UseBorders = false;
            // 
            // xrTableRow2
            // 
            this.xrTableRow2.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell4,
            this.xrTableCell5,
            this.xrTableCell6});
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.StylePriority.UseBorders = false;
            this.xrTableRow2.Weight = 1D;
            // 
            // xrTableCell4
            // 
            this.xrTableCell4.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell4.Name = "xrTableCell4";
            this.xrTableCell4.StylePriority.UseBorders = false;
            this.xrTableCell4.Text = "xrTableCell1";
            this.xrTableCell4.Weight = 1D;
            // 
            // xrTableCell5
            // 
            this.xrTableCell5.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell5.Name = "xrTableCell5";
            this.xrTableCell5.StylePriority.UseBorders = false;
            this.xrTableCell5.Text = "xrTableCell2";
            this.xrTableCell5.Weight = 1D;
            // 
            // xrTableCell6
            // 
            this.xrTableCell6.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrTableCell6.Name = "xrTableCell6";
            this.xrTableCell6.StylePriority.UseBorders = false;
            this.xrTableCell6.Text = "xrTableCell3";
            this.xrTableCell6.Weight = 1D;
            // 
            // xrPanel1
            // 
            this.xrPanel1.BackColor = System.Drawing.Color.White;
            this.xrPanel1.BorderWidth = 0.5F;
            this.xrPanel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 74.0834F);
            this.xrPanel1.Name = "xrPanel1";
            this.xrPanel1.SizeF = new System.Drawing.SizeF(1070F, 15F);
            this.xrPanel1.StylePriority.UseBackColor = false;
            this.xrPanel1.StylePriority.UseBorderWidth = false;
            // 
            // xrLabel5
            // 
            this.xrLabel5.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10.00001F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(45F, 14F);
            this.xrLabel5.StyleName = "FieldCaption";
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.Text = "Class : ";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel6
            // 
            this.xrLabel6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ClassDiv]")});
            this.xrLabel6.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(45F, 10.00001F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(535.6668F, 14.99998F);
            this.xrLabel6.StyleName = "DataField";
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.Text = "xrLabel4";
            this.xrLabel6.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.xrLabel6_BeforePrint);
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.Constructor = objectConstructorInfo1;
            this.objectDataSource1.DataSourceType = null;
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // GroupFooter1
            // 
            this.GroupFooter1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel26});
            this.GroupFooter1.HeightF = 14.58333F;
            this.GroupFooter1.Name = "GroupFooter1";
            this.GroupFooter1.PrintAtBottom = true;
            // 
            // GroupHeader2
            // 
            this.GroupHeader2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel3,
            this.xrLabel4,
            this.xrLabel8,
            this.xrLabel9});
            this.GroupHeader2.HeightF = 76.04166F;
            this.GroupHeader2.Level = 2;
            this.GroupHeader2.Name = "GroupHeader2";
            this.GroupHeader2.RepeatEveryPage = true;
            // 
            // xrLabel3
            // 
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[SchoolName]")});
            this.xrLabel3.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(1070F, 25F);
            this.xrLabel3.StyleName = "DataField";
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel4
            // 
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[SchoolAddress]")});
            this.xrLabel4.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel4.ForeColor = System.Drawing.Color.Black;
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(0F, 25.99999F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(1070F, 14F);
            this.xrLabel4.StyleName = "FieldCaption";
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UseForeColor = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel8
            // 
            this.xrLabel8.BackColor = System.Drawing.Color.White;
            this.xrLabel8.BorderColor = System.Drawing.Color.Black;
            this.xrLabel8.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel8.BorderWidth = 1F;
            this.xrLabel8.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold);
            this.xrLabel8.ForeColor = System.Drawing.Color.Black;
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(0F, 41F);
            this.xrLabel8.Multiline = true;
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(1070F, 14.25002F);
            this.xrLabel8.StyleName = "Title";
            this.xrLabel8.StylePriority.UseBackColor = false;
            this.xrLabel8.StylePriority.UseBorderColor = false;
            this.xrLabel8.StylePriority.UseBorders = false;
            this.xrLabel8.StylePriority.UseBorderWidth = false;
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UseForeColor = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.Text = "Monthly Attendance Report";
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel9
            // 
            this.xrLabel9.BackColor = System.Drawing.Color.White;
            this.xrLabel9.BorderColor = System.Drawing.Color.Black;
            this.xrLabel9.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel9.BorderWidth = 1F;
            this.xrLabel9.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters].[FromDate]")});
            this.xrLabel9.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold);
            this.xrLabel9.ForeColor = System.Drawing.Color.Black;
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(0F, 58.25002F);
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(1070F, 14.25001F);
            this.xrLabel9.StyleName = "Title";
            this.xrLabel9.StylePriority.UseBackColor = false;
            this.xrLabel9.StylePriority.UseBorderColor = false;
            this.xrLabel9.StylePriority.UseBorders = false;
            this.xrLabel9.StylePriority.UseBorderWidth = false;
            this.xrLabel9.StylePriority.UseFont = false;
            this.xrLabel9.StylePriority.UseForeColor = false;
            this.xrLabel9.StylePriority.UseTextAlignment = false;
            this.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel9.TextFormatString = "{0:dd-MM-yyyy}";
            this.xrLabel9.WordWrap = false;
            // 
            // Monthly_Attendance
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.groupHeaderBand1,
            this.pageFooterBand1,
            this.reportHeaderBand1,
            this.GroupHeader1,
            this.GroupFooter1,
            this.GroupHeader2});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objectDataSource1});
            this.DataSource = this.objectDataSource1;
            this.Landscape = true;
            this.Margins = new System.Drawing.Printing.Margins(15, 15, 15, 15);
            this.PageHeight = 850;
            this.PageWidth = 1100;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.FromDate,
            this.Todate,
            this.printby});
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.Title,
            this.FieldCaption,
            this.PageInfo,
            this.DataField});
            this.Version = "18.1";
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

    }

    DataTable dt, dt1;
    public XRTable CreateXRTable(DataTable dt)
    {

        XRTable table = xrTable1 as XRTable;
        XRTable table2 = xrTable2 as XRTable;


        table2.Rows.Clear();
        table.Rows.Clear();
        if (dt.Rows.Count > 0)
        {
            int columnstart = Convert.ToInt32(dt.Rows[0]["MonthRepostartday"]);// 3;
            int columnInRow = Convert.ToInt32(dt.Rows[0]["LastdayMonth"]);//3;
            //int rowsCount = 8;
            float rowHeight = 15f;

            table.Borders = DevExpress.XtraPrinting.BorderSide.All;
            table.BorderWidth = 0.5f;
            table.BeginInit();

            table2.Borders = DevExpress.XtraPrinting.BorderSide.All;
            table2.BorderWidth = 0.5f;
            table2.BeginInit();

            #region Header Row
            XRTableRow Headerrow = new XRTableRow();
            Headerrow.HeightF = rowHeight;

            //XRTableCell cellDepartment = new XRTableCell();
            //cellDepartment.Font = new System.Drawing.Font("Times New Roman", 6F, System.Drawing.FontStyle.Bold);
            //cellDepartment.BackColor = System.Drawing.Color.LightSteelBlue;
            //cellDepartment.Name = "cellDepartment";
            //cellDepartment.StylePriority.UseFont = false;
            //cellDepartment.StylePriority.UseTextAlignment = false;
            //cellDepartment.Text = "Department";
            //cellDepartment.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            //cellDepartment.Weight = 0.4D;
            //Headerrow.Cells.Add(cellDepartment);

            XRTableCell cellEmpcode = new XRTableCell();
            cellEmpcode.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold);
            cellEmpcode.BackColor = System.Drawing.Color.LightSteelBlue;
            cellEmpcode.Name = "cellEmpcode";
            cellEmpcode.StylePriority.UseFont = false;
            cellEmpcode.StylePriority.UseTextAlignment = false;
            cellEmpcode.Text = "PunchID";
            cellEmpcode.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            cellEmpcode.Weight = 1D;
            Headerrow.Cells.Add(cellEmpcode);


            XRTableCell cellGrno = new XRTableCell();
            cellGrno.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold);
            cellGrno.BackColor = System.Drawing.Color.LightSteelBlue;
            cellGrno.Name = "cellGrno";
            cellGrno.StylePriority.UseFont = false;
            cellGrno.StylePriority.UseTextAlignment = false;
            cellGrno.Text = "G.R.No";
            cellGrno.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            cellGrno.Weight = 1D;
            Headerrow.Cells.Add(cellGrno);


            XRTableCell cellEmpName = new XRTableCell();
            cellEmpName.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold);
            cellEmpName.BackColor = System.Drawing.Color.LightSteelBlue;
            cellEmpName.Name = "cellEmpName";
            cellEmpName.StylePriority.UseFont = false;
            cellEmpName.StylePriority.UseTextAlignment = false;
            cellEmpName.Text = "Name";
            cellEmpName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            cellEmpName.Weight = 1.5D;
            Headerrow.Cells.Add(cellEmpName);



            int k = columnstart;
            for (int j = 1; j <= columnInRow; j++)
            {

                XRTableCell celldynamic = new XRTableCell();
                string fname = k.ToString();
                string cellname = "cell" + k.ToString();
                celldynamic.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold);
                celldynamic.BackColor = System.Drawing.Color.LightSteelBlue;
                celldynamic.Name = cellname;
                celldynamic.StylePriority.UseFont = false;
                celldynamic.StylePriority.UseTextAlignment = false;
                celldynamic.Text = fname;
                celldynamic.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                celldynamic.Weight = 0.44900344848632823D;
                Headerrow.Cells.Add(celldynamic);
                if (k == columnInRow)
                {
                    k = 1;
                }
                else
                {
                    k++;
                }
            }


            XRTableCell cellP = new XRTableCell();
            cellP.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold);
            cellP.BackColor = System.Drawing.Color.LightSteelBlue;
            cellP.Name = "cellP";
            cellP.StylePriority.UseFont = false;
            cellP.StylePriority.UseTextAlignment = false;
            cellP.Text = "P";
            cellP.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            Headerrow.Cells.Add(cellP);

            XRTableCell cellA = new XRTableCell();
            cellA.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold);
            cellA.BackColor = System.Drawing.Color.LightSteelBlue;
            cellA.Name = "cellA";
            cellA.StylePriority.UseFont = false;
            cellA.StylePriority.UseTextAlignment = false;
            cellA.Text = "A";
            cellA.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            Headerrow.Cells.Add(cellA);

            XRTableCell cellWO = new XRTableCell();
            cellWO.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold);
            cellWO.BackColor = System.Drawing.Color.LightSteelBlue;
            cellWO.Name = "cellWO";
            cellWO.StylePriority.UseFont = false;
            cellWO.StylePriority.UseTextAlignment = false;
            cellWO.Text = "WO";
            cellWO.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            Headerrow.Cells.Add(cellWO);

            XRTableCell cellH = new XRTableCell();
            cellH.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold);
            cellH.BackColor = System.Drawing.Color.LightSteelBlue;
            cellH.Name = "cellH";
            cellH.StylePriority.UseFont = false;
            cellH.StylePriority.UseTextAlignment = false;
            cellH.Text = "HO";
            cellH.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            Headerrow.Cells.Add(cellH);

            table2.Rows.Add(Headerrow);

            table2.AdjustSize();
            table2.EndInit();
            //table2.LocationF = new DevExpress.Utils.PointFloat(0F, 0F);
            table2.WidthF = this.PageWidth - this.Margins.Left - this.Margins.Right;

            #endregion

            #region Detail Row
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                XRTableRow detailrow = new XRTableRow();
                detailrow.HeightF = rowHeight;

                XRTableCell celldetailEmpcode = new XRTableCell();
                celldetailEmpcode.Font = new System.Drawing.Font("Times New Roman", 6F);
                celldetailEmpcode.Name = "celldetailEmpcode" + i.ToString();
                celldetailEmpcode.StylePriority.UseFont = false;
                celldetailEmpcode.StylePriority.UseTextAlignment = false;
                celldetailEmpcode.Text = dt.Rows[i]["PunchID"].ToString();
                celldetailEmpcode.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                celldetailEmpcode.Weight = 1D;
                detailrow.Cells.Add(celldetailEmpcode);

                XRTableCell celldetailGrNo = new XRTableCell();
                celldetailGrNo.Font = new System.Drawing.Font("Times New Roman", 6F);
                celldetailGrNo.Name = "celldetailGrNo" + i.ToString();
                celldetailGrNo.StylePriority.UseFont = false;
                celldetailGrNo.StylePriority.UseTextAlignment = false;
                celldetailGrNo.Text = dt.Rows[i]["GRNo"].ToString();
                celldetailGrNo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                celldetailGrNo.Weight = 1D;
                detailrow.Cells.Add(celldetailGrNo);

                XRTableCell celldetailEmpName = new XRTableCell();
                celldetailEmpName.Font = new System.Drawing.Font("Times New Roman", 7F);
                celldetailEmpName.Name = "celldetailEmpName" + i.ToString();
                celldetailEmpName.StylePriority.UseFont = false;
                celldetailEmpName.StylePriority.UseTextAlignment = false;
                celldetailEmpName.Text = dt.Rows[i]["Stu_Name"].ToString();
                celldetailEmpName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                celldetailEmpName.Weight = 1.5D;
                detailrow.Cells.Add(celldetailEmpName);



                int cs = columnstart;
                
                for (int j = 1; j <= columnInRow; j++)
                {
                    string colname = "D" + cs.ToString();
                    string cellname = "celldetaildynamic" + cs.ToString();
                    string coltext = "";

                    if (dt.Columns.Contains(colname))
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i][colname].ToString()) == false)
                        {
                            coltext = dt.Rows[i][colname].ToString();
                        }
                    }

                    XRTableCell celldetaildynamic = new XRTableCell();
                    celldetaildynamic.Font = new System.Drawing.Font("Times New Roman", 7F);
                    celldetaildynamic.Name = cellname;
                    celldetaildynamic.StylePriority.UseFont = false;
                    celldetaildynamic.StylePriority.UseTextAlignment = false;
                    celldetaildynamic.Text = coltext;
                    celldetaildynamic.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                    celldetaildynamic.Weight = 0.44900344848632823D;
                    detailrow.Cells.Add(celldetaildynamic);

                    if (cs == columnInRow)
                    {
                        cs = 1;
                    }
                    else
                    {
                        cs++;
                    }



                }

                XRTableCell celldetailP = new XRTableCell();
                celldetailP.Font = new System.Drawing.Font("Times New Roman", 7F);
                celldetailP.Name = "celldetailP" + i.ToString();
                celldetailP.StylePriority.UseFont = false;
                celldetailP.StylePriority.UseTextAlignment = false;
                celldetailP.Text = dt.Rows[i]["P"].ToString();
                celldetailP.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                detailrow.Cells.Add(celldetailP);


                XRTableCell celldetailA = new XRTableCell();
                celldetailA.Font = new System.Drawing.Font("Times New Roman", 7F);
                celldetailA.Name = "celldetailA" + i.ToString();
                celldetailA.StylePriority.UseFont = false;
                celldetailA.StylePriority.UseTextAlignment = false;
                celldetailA.Text = dt.Rows[i]["A"].ToString();
                celldetailA.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                detailrow.Cells.Add(celldetailA);

                XRTableCell celldetailWO = new XRTableCell();
                celldetailWO.Font = new System.Drawing.Font("Times New Roman", 7F);
                celldetailWO.Name = "celldetailWO" + i.ToString();
                celldetailWO.StylePriority.UseFont = false;
                celldetailWO.StylePriority.UseTextAlignment = false;
                celldetailWO.Text = dt.Rows[i]["W"].ToString();
                celldetailWO.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                detailrow.Cells.Add(celldetailWO);

                XRTableCell celldetailH = new XRTableCell();
                celldetailH.Font = new System.Drawing.Font("Times New Roman", 7F);
                celldetailH.Name = "celldetailH" + i.ToString();
                celldetailH.StylePriority.UseFont = false;
                celldetailH.StylePriority.UseTextAlignment = false;
                celldetailH.Text = dt.Rows[i]["H"].ToString();
                celldetailH.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                detailrow.Cells.Add(celldetailH);

                table.Rows.Add(detailrow);
            }


            #endregion

            table.AdjustSize();
            table.EndInit();
            table.LocationF = new DevExpress.Utils.PointFloat(0F, 0F);
            table.WidthF = this.PageWidth - this.Margins.Left - this.Margins.Right;


        }


        return table;
    }

    #endregion

    private void xrLabel6_BeforePrint(object sender, PrintEventArgs e)
    {
        string expr = "ClassDiv='" + GetCurrentColumnValue("ClassDiv") + "'";
        string shortorder = "Stu_Name ASC";
        DataTable dt = this.DataSource as DataTable;

        if (dt.Rows.Count > 0)
        {
            dt1 = dt.Clone();

            foreach (DataRow dr in dt.Select(expr, shortorder))
            {
                dt1.ImportRow(dr);
            }
            this.groupHeaderBand1.Controls.Add(CreateXRTable(dt1));
        }
    }










}
