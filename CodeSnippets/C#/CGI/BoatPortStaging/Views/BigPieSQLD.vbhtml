@ModelType IPrime.Web.PortStagingFinalReportModel
@Code
    ViewData("Title") = "BigPieSQLD"
End Code

<h2>Port Staging Comparison Report</h2>

<style type="text/css">
    .tableCount {
        z-index: 1;
        position: relative;
    }

    table#tableCount td, table#tableCount, table#tableCount tr {
        padding: 20px;
        font-weight: bold;
    }

    .linkStyle {
        text-decoration: none;
        font-weight: bold;
    }

        .linkStyle:hover {
            text-decoration: underline;
            opacity: 0.8;
        }
</style>

<script src="~/Scripts/jqPlot/jquery.jqplot.js"></script>
<script src="~/Scripts/jqPlot/plugins/jqplot.pieRenderer.js"></script>
<link rel="stylesheet" type="text/css" href="~/Scripts/jquery.jqplot.css" />
<script type="text/javascript">

    $(document).ready(function () {

        if ('@Model.InventoryItemsSqldStaging.Inventory_FinalStagedCount' != '0' || '@Model.InventoryItemsSqldStaging.Inventory_NotStagedCount' != '0') {
            var plot1 = $.jqplot('bigPieSQLD', [[['', parseInt('@Model.InventoryItemsSqldStaging.Inventory_FinalStagedCount')], ['', parseInt('@Model.InventoryItemsSqldStaging.Inventory_NotStagedCount')]]], {
                grid: {
                    drawBorder: false,
                    drawGridlines: false,
                    background: '#ffffff',
                    shadow: false
                },
                gridPadding: { top: 0, bottom: 38, left: 0, right: 0 },
                seriesDefaults: {
                    renderer: $.jqplot.PieRenderer,
                    trendline: { show: false },
                    rendererOptions: {
                        padding: 8,
                        showDataLabels: true,
                        dataLabelThreshold: 0.1,
                        dataLabelFormatString: '%.1f%%',
                        highlightMouseDown: false,
                        highlightColor: null,
                    }
                },
                legend: {
                    show: true,
                    placement: 'outside',
                    rendererOptions: {
                        numberRows: 1
                    },
                    location: 's',
                    marginTop: '15px'
                },
                seriesColors: [
                    '#3377aa', '#aa2211'
                ]
            });
        }

            $('#bigPieSQLD').bind('jqplotDataClick', function (ev, seriesIndex, pointIndex, data) {
                var colorSelection = plot1.series[seriesIndex].seriesColors[pointIndex]

                if (colorSelection === '#3377aa') {
                    $("#BigPieStagedSQLDActionBtn").click();
                }
                else {
                    $("#BigPieNotStagedSQLDActionBtn").click();
                }
            });
        });

        function StagedSQLD() {
            $("#BigPieStagedSQLDActionBtn").click();
        }

        function NotStagedSQLD() {
            $("#BigPieNotStagedSQLDActionBtn").click();
        }

        function StagedSinceLastPacmanSQLD() {
            $("#BigPieStagedSinceLastPacmandSQLDActionBtn").click();
        }

        function RemovedSQLD() {
            $("#BigPieRemovedSQLDActionBtn").click();
        }

        function BackToList() {
            $("#BackToListActionBtn").click();
        }

        function ExportExcel() {
            $("#ExportExcelActionBtn").click();
        }

</script>

<table>
    <tr><td><div><input type="button" id="ExportExcel" value="Export" onclick=ExportExcel() /></div></td></tr>
    <tr></tr>
    <tr><td><div><a href="javascript:BackToList();" class="linkStyle" style="text-decoration:underline;">Back To List</a></div></td></tr>
</table>

<table>
    <tr>
        <td>
            <h3 style="border:none; margin:5px 0 0 0; color:black; font-weight:bold; ">@Model.InventoryName SQLD STAGING</h3>
            <h3 style="border:none; margin-top:3px; margin-bottom:15px; font-weight:bold; ">Planned RORO Load Is @Model.InventoryItemsSqldStaging.Inventory_TotalCount Items</h3>
            <div style="overflow:auto;">
                <div id="bigPieSQLD" style="float:left; font-weight:800; color:black; width:520px; height:520px;"></div>
            </div>
        </td>
        <td style="padding-right:10px;">
            <div style="float:left; padding-top:20px; width:180px;">
                <table id="tableCount" class="tableCount">
                    <tr>
                        <td><a href="javascript:StagedSQLD();" class="linkStyle">Staged (@Model.InventoryItemsSqldStaging.Inventory_FinalStagedCount)</a></td>
                    </tr>
                    <tr>
                        <td><a href="javascript:NotStagedSQLD();" class="linkStyle">Not Staged (@Model.InventoryItemsSqldStaging.Inventory_NotStagedCount)</a></td>
                    </tr>
                    <tr>
                        <td><a href="javascript:StagedSinceLastPacmanSQLD();" class="linkStyle">Staged Since Last Inventory (@Model.InventoryItemsSqldStaging.Inventory_StagedSinceLastPacmanCount)</a></td>
                    </tr>
                    <tr>
                        <td><a href="javascript:RemovedSQLD();" class="linkStyle">Removed Since Last Inventory (@Model.InventoryItemsSqldStaging.Inventory_RemovedSinceLastPacmanCount)</a></td>
                    </tr>
                </table>
            </div>
        </td>
        <td>
            <div style="overflow:auto; float:right; ">
                <label>@Model.CurrentStageSelectionHeader</label>
                @If Model.ReportItems IsNot Nothing Then
                    @If Model.ReportItems.Any Then
                        @Using Html.BeginForm
                            Dim grid As New WebGrid(Model.ReportItems, Nothing, Nothing, Model.SelectedPageSize, True, True)

                            @grid.Table(tableStyle:="gridview autopost",
                                        rowStyle:="row",
                                        alternatingRowStyle:="arow",
                                        headerStyle:="header",
                                        footerStyle:="footer",
                                        columns:=grid.Columns(grid.Column(columnName:="ItemId", header:="Item ID", canSort:=True),
                                                              grid.Column(columnName:="NSN", header:="NSN", canSort:=True),
                                                              grid.Column(columnName:="SerialNumber", header:="Serial", canSort:=True),
                                                              grid.Column(columnName:="PRI", header:="PRI", canSort:=True),
                                                              grid.Column(columnName:="Nomen", header:="Nomenclature", canSort:=True),
                                                              grid.Column(columnName:="Location", header:="Location", canSort:=True),
                                                              grid.Column(columnName:="IsFound", header:="Found", canSort:=True),
                                                              grid.Column(columnName:="Modified", header:="Modified", canSort:=True, format:=Function(item) If(item.Modified.ToString = "00:00:00", String.Format(""), String.Format(item.Modified.ToString)))))

                            @<div>
                                @grid.Pager(WebGridPagerModes.All)
                                <div style="float: right; text-align: right;">
                                    @grid.Rows.Count of @grid.TotalRowCount
                                </div>
                            </div>
                        End Using
                    Else
                        @<p style="padding-bottom:500px;">There are no items to display.</p>
                    End If
                End If
            </div>
        </td>
    </tr>
</table>

@Using (Html.BeginForm("BigPieStagedSQLD", "PortStagingFinalReport", FormMethod.Post))
    @Html.HiddenFor(Function(model) model.ShowReport)
    @Html.HiddenFor(Function(model) model.SelectedInventoryIdA)
    @Html.HiddenFor(Function(model) model.SelectedInventoryIdB)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_FinalStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_NotStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_StagedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_RemovedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_TotalCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_FinalStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_NotStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_StagedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_RemovedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_TotalCount)
     @Html.HiddenFor(Function(model) model.InventoryName)
    @<input type="submit" hidden="hidden" id="BigPieStagedSQLDActionBtn" value="" />
End Using

@Using (Html.BeginForm("BigPieNotStagedSQLD", "PortStagingFinalReport", FormMethod.Post))
    @Html.HiddenFor(Function(model) model.ShowReport)
    @Html.HiddenFor(Function(model) model.SelectedInventoryIdA)
    @Html.HiddenFor(Function(model) model.SelectedInventoryIdB)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_FinalStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_NotStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_StagedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_RemovedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_TotalCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_FinalStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_NotStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_StagedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_RemovedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_TotalCount)
     @Html.HiddenFor(Function(model) model.InventoryName)
    @<input type="submit" hidden="hidden" id="BigPieNotStagedSQLDActionBtn" value="" />
End Using

@Using (Html.BeginForm("BigPieStagedSinceLastPacmanSQLD", "PortStagingFinalReport", FormMethod.Post))
    @Html.HiddenFor(Function(model) model.ShowReport)
    @Html.HiddenFor(Function(model) model.SelectedInventoryIdA)
    @Html.HiddenFor(Function(model) model.SelectedInventoryIdB)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_FinalStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_NotStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_StagedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_RemovedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_TotalCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_FinalStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_NotStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_StagedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_RemovedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_TotalCount)
     @Html.HiddenFor(Function(model) model.InventoryName)
    @<input type="submit" hidden="hidden" id="BigPieStagedSinceLastPacmandSQLDActionBtn" value="" />
End Using

@Using (Html.BeginForm("BigPieRemovedSQLD", "PortStagingFinalReport", FormMethod.Post))
    @Html.HiddenFor(Function(model) model.ShowReport)
    @Html.HiddenFor(Function(model) model.SelectedInventoryIdA)
    @Html.HiddenFor(Function(model) model.SelectedInventoryIdB)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_FinalStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_NotStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_StagedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_RemovedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_TotalCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_FinalStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_NotStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_StagedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_RemovedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_TotalCount)
     @Html.HiddenFor(Function(model) model.InventoryName)
    @<input type="submit" hidden="hidden" id="BigPieRemovedSQLDActionBtn" value="" />
End Using

@Using (Html.BeginForm("BackToList", "PortStagingFinalReport", FormMethod.Post))
    @Html.HiddenFor(Function(model) model.ShowReport)
    @Html.HiddenFor(Function(model) model.SelectedInventoryIdA)
    @Html.HiddenFor(Function(model) model.SelectedInventoryIdB)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_FinalStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_NotStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_StagedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_RemovedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_TotalCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_FinalStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_NotStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_StagedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_RemovedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_TotalCount)
     @Html.HiddenFor(Function(model) model.InventoryName)
    @<input type="submit" hidden="hidden" id="BackToListActionBtn" value="" />
End Using

@Using (Html.BeginForm("ExportExcelMethod", "PortStagingFinalReport", FormMethod.Post))
    @Html.HiddenFor(Function(model) model.ShowReport)
    @Html.HiddenFor(Function(model) model.SelectedInventoryIdA)
    @Html.HiddenFor(Function(model) model.SelectedInventoryIdB)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_FinalStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_NotStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_StagedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_RemovedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsLoloStaged.Inventory_TotalCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_FinalStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_NotStagedCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_StagedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_RemovedSinceLastPacmanCount)
    @Html.HiddenFor(Function(model) model.InventoryItemsSqldStaging.Inventory_TotalCount)
     @Html.HiddenFor(Function(model) model.InventoryName)
    @<input type="submit" hidden="hidden" id="ExportExcelActionBtn" value="" />
End Using