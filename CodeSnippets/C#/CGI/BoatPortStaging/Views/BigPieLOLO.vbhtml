@ModelType IPrime.Web.PortStagingFinalReportModel
@Code
    ViewData("Title") = "BigPieLOLO"
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
        font-weight:bold;
    }

    .linkStyle:hover {
        text-decoration: underline;
        opacity:0.8;
    }
</style>

<script src="~/Scripts/jqPlot/jquery.jqplot.js"></script>
<script src="~/Scripts/jqPlot/plugins/jqplot.pieRenderer.js"></script>
<link rel="stylesheet" type="text/css" href="~/Scripts/jquery.jqplot.css" />
<script type="text/javascript">

    $(document).ready(function () {

        if ('@Model.InventoryItemsLoloStaged.Inventory_FinalStagedCount' != '0' || '@Model.InventoryItemsLoloStaged.Inventory_NotStagedCount' != '0') {
            var plot1 = $.jqplot('bigPieLOLO', [[['', parseInt('@Model.InventoryItemsLoloStaged.Inventory_FinalStagedCount')], ['', parseInt('@Model.InventoryItemsLoloStaged.Inventory_NotStagedCount')]]], {
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

            $('#bigPieLOLO').bind('jqplotDataClick', function (ev, seriesIndex, pointIndex, data) {
                var colorSelection = plot1.series[seriesIndex].seriesColors[pointIndex]

                if (colorSelection === '#3377aa') {
                    $("#BigPieStagedLOLOActionBtn").click();
                }
                else {
                    $("#BigPieNotStagedLOLOActionBtn").click();
                }
            });
        });

        function StagedLOLO() {
            $("#BigPieStagedLOLOActionBtn").click();
        }

        function NotStagedLOLO() {
            $("#BigPieNotStagedLOLOActionBtn").click();
        }

        function StagedSinceLastPacmanLOLO() {
            $("#BigPieStagedSinceLastPacmandLOLOActionBtn").click();
        }

        function RemovedLOLO() {
            $("#BigPieRemovedLOLOActionBtn").click();
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
            <h3 style="border:none; margin:5px 0 0 0; color:black; font-weight:bold; ">@Model.InventoryName LOLO STAGED</h3>
            <h3 style="border:none; margin-top:3px; margin-bottom:15px; font-weight:bold; ">Planned LOLO (Without Ammo) Load Is @Model.InventoryItemsLoloStaged.Inventory_TotalCount Items</h3>
            <div style="overflow:auto;">
                <div id="bigPieLOLO" style="float:left; font-weight:800; color:black; width:520px; height:520px;"></div>
            </div>
        </td>
        <td style="padding-right:10px;">
            <div style="float:left; padding-top:20px; width:180px;">
                <table id="tableCount" class="tableCount">
                    <tr>
                        <td><a href="javascript:StagedLOLO();" class="linkStyle">Staged (@Model.InventoryItemsLoloStaged.Inventory_FinalStagedCount)</a></td>
                    </tr>
                    <tr>
                        <td><a href="javascript:NotStagedLOLO();" class="linkStyle">Not Staged (@Model.InventoryItemsLoloStaged.Inventory_NotStagedCount)</a></td>
                    </tr>
                    <tr>
                        <td><a href="javascript:StagedSinceLastPacmanLOLO();" class="linkStyle">Staged Since Last Inventory (@Model.InventoryItemsLoloStaged.Inventory_StagedSinceLastPacmanCount)</a></td>
                    </tr>
                    <tr>
                        <td><a href="javascript:RemovedLOLO();" class="linkStyle">Removed Since Last Inventory (@Model.InventoryItemsLoloStaged.Inventory_RemovedSinceLastPacmanCount)</a></td>
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

@Using (Html.BeginForm("BigPieStagedLOLO", "PortStagingFinalReport", FormMethod.Post))
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
    @<input type="submit" hidden="hidden" id="BigPieStagedLOLOActionBtn" value="" />
End Using

@Using (Html.BeginForm("BigPieNotStagedLOLO", "PortStagingFinalReport", FormMethod.Post))
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
    @<input type="submit" hidden="hidden" id="BigPieNotStagedLOLOActionBtn" value="" />
End Using

@Using (Html.BeginForm("BigPieStagedSinceLastPacmanLOLO", "PortStagingFinalReport", FormMethod.Post))
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
    @<input type="submit" hidden="hidden" id="BigPieStagedSinceLastPacmandLOLOActionBtn" value="" />
End Using

@Using (Html.BeginForm("BigPieRemovedLOLO", "PortStagingFinalReport", FormMethod.Post))
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
    @<input type="submit" hidden="hidden" id="BigPieRemovedLOLOActionBtn" value="" />
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