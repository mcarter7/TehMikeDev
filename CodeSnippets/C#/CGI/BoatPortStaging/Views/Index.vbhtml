@ModelType IPrime.Web.PortStagingFinalReportModel
@Code
    ViewData("Title") = "Index"
End Code

<style type="text/css">
    .tableCount {
        z-index: 1;
        position:relative; 
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

    .loading {
        position: absolute;
        background-color: #FAFAFA;
        z-index: 2147483647 !important;
        opacity: 0.8;
        overflow: hidden;
        text-align: center;
        top: 0;
        left: 0;
        vertical-align: central;
        height: 200%;
        width: 100%;
        padding-top: 17%;
    }

</style>

<script src="~/Scripts/jqPlot/jquery.jqplot.js"></script>
<script src="~/Scripts/jqPlot/plugins/jqplot.pieRenderer.js"></script>
<link rel="stylesheet" type="text/css" href="~/Scripts/jquery.jqplot.css" />
<script type="text/javascript">

    function CompareBtnClick() {
        var dropBoxValueA = $("#StagingInventoryDropBoxA")[0].value;
        var dropBoxValueB = $("#StagingInventoryDropBoxB")[0].value;

        var modifiedDateA = Date.parse(dropBoxValueA.toString().substring(37))
        var modifiedDateB = Date.parse(dropBoxValueB.toString().substring(37))
        console.log("Date A: " + modifiedDateA)
        console.log("Date B: " + modifiedDateB)

        if (dropBoxValueA != "" && dropBoxValueB != "") {
            if (modifiedDateA > modifiedDateB) {
                $('<div></div>').appendTo('body')
                .html('<div><h6 style="font-size:12px">The previous inventory has a modified date later than the other inventory. Are you sure you want to continue the comparison?</h6></div>')
                .dialog({
                    modal: true,
                    open: function (event, ui) {
                        $(".ui-dialog-titlebar-close").hide()
                    },
                    zIndex: 10000,
                    autoOpen: true,
                    width: 'auto',
                    resizable: false,
                    buttons: {
                        Yes: function () {
                            $("#loadingDiv").attr('hidden', false)
                            var url = '@Url.Action("CompareBtnClick")';
                            location.href = url + "?dropBoxValueA=" + dropBoxValueA + "&dropBoxValueB=" + dropBoxValueB
                            $(this).dialog("close");
                        },
                        No: function () {
                            $(this).dialog("close");
                        }
                    },
                    close: function (event, ui) {
                        $(this).remove();
                    }
                });
            }

            if (modifiedDateA < modifiedDateB) {
                $("#loadingDiv").attr('hidden', false)
                var url = '@Url.Action("CompareBtnClick")';
                location.href = url + "?dropBoxValueA=" + dropBoxValueA + "&dropBoxValueB=" + dropBoxValueB
            }
        }
        else { alert("Two inventories must be selected!"); }
    }

    $(document).ready(function () {

        $("#loadingDiv").attr('hidden', true)

        if ('@Model.ShowReport' === 'True') {

            if ('@Model.InventoryItemsLoloStaged.Inventory_FinalStagedCount' != '0' || '@Model.InventoryItemsLoloStaged.Inventory_NotStagedCount' != '0') {

                var plot1 = $.jqplot('pie1', [[['', parseInt('@Model.InventoryItemsLoloStaged.Inventory_FinalStagedCount')], ['', parseInt('@Model.InventoryItemsLoloStaged.Inventory_NotStagedCount')]]], {
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

            if ('@Model.InventoryItemsSqldStaging.Inventory_FinalStagedCount' != '0' || '@Model.InventoryItemsSqldStaging.Inventory_NotStagedCount' != '0') {
                var plot2 = $.jqplot('pie2', [[['', parseInt('@Model.InventoryItemsSqldStaging.Inventory_FinalStagedCount')], ['', parseInt('@Model.InventoryItemsSqldStaging.Inventory_NotStagedCount')]]], {
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

                $('#pie1').bind('jqplotDataClick', function (ev, seriesIndex, pointIndex, data) {
                    var colorSelection = plot1.series[seriesIndex].seriesColors[pointIndex]

                    if (colorSelection === '#3377aa') {
                        $("#BigPieStagedLOLOActionBtn").click();
                    }
                    else {
                        $("#BigPieNotStagedLOLOActionBtn").click();
                    }
                });

                $('#pie2').bind('jqplotDataClick', function (ev, seriesIndex, pointIndex, data) {
                    var colorSelection = plot2.series[seriesIndex].seriesColors[pointIndex]

                    if (colorSelection === '#3377aa') {
                        $("#BigPieStagedSQLDActionBtn").click();
                    }
                    else {
                        $("#BigPieNotStagedSQLDActionBtn").click();
                    }
                });
            }

        }
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

    function ExportExcel() {
        $("#ExportExcelActionBtn").click();
    }

</script>

<div id="loadingDiv" class="loading" hidden="hidden">Loading...</div>

<h2>Port Staging Comparison Report</h2>

@If Not Model.ShowReport Then
    If Not Model.NoInventoriesToCompare Then
    @<fieldset>
        <legend>Select Inventories For Comparison Report</legend>

        <table>
            <tr>
                <td style="font-weight:bold">Previous</td>
                <td></td>
                <td style="font-weight:bold">Current</td>
            </tr>
            <tr>
                <td>@Html.DropDownList("StagingInventoryDropBoxA", "--Select--")</td>

                <td>AND</td>

                <td>@Html.DropDownList("StagingInventoryDropBoxB", "--Select--")</td>

                <td> <input type="button" id="comparisonBtn" value="Compare" onclick=CompareBtnClick() /> </td>
            </tr>
        </table>
    </fieldset>
Else
    @<p>No port staging inventories to compare</p>
End If
End If

@If Model.ShowReport Then
    @<div><input type="button" id="ExportExcel" value="Export" onclick=ExportExcel() /></div>
    
    @<table>
        <tr>
            <td>
                <h3 style="border:none; margin:5px 0 0 0; color:black; font-weight:bold; ">@Model.InventoryName LOLO STAGED</h3>
                <h3 style="border:none; margin-top:3px; margin-bottom:15px; font-weight:bold; ">Planned LOLO (Without Ammo) Load Is @Model.InventoryItemsLoloStaged.Inventory_TotalCount Items</h3>
                <div style="overflow:auto; ">
                    <div id="pie1" style="float:left; font-weight:800; color:black; width:320px; height:320px;"></div>
                </div>
            </td>
            <td style="padding-right:80px;">
                <div style="float:left; margin-left:-65px; padding-top:1px;">
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
                <h3 style="border:none; margin:5px 0 0 0; color:black; font-weight:bold; ">@Model.InventoryName SQLD STAGING</h3>
                <h3 style="border:none; margin-top:3px; margin-bottom:15px; font-weight:bold; ">Planned RORO Load Is @Model.InventoryItemsSqldStaging.Inventory_TotalCount Items</h3>
                <div style="overflow:auto;">
                    <div id="pie2" style="float:left; font-weight:800; color:black; width:320px; height:320px;"></div>
                </div>
            </td>
            <td style="padding-right:80px;">
                <div style="float:left; margin-left:-10px;">
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
        </tr>
    </table>

End If

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

@Using (Html.BeginForm("ExportExcelMethod", "PortStagingFinalReport", FormMethod.Post))
    @Html.HiddenFor(Function(model) model.ShowReport)
    @Html.HiddenFor(Function(model) model.InventoryName)
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
    @<input type="submit" hidden="hidden" id="ExportExcelActionBtn" value="" />
End Using