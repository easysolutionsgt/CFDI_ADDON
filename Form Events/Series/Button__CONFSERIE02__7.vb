'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports B1WizardBase
Imports SAPbobsCOM
Imports SAPbouiCOM

Namespace Factura_Electronica
    
    Public Class Button__CONFSERIE02__7
        Inherits B1Item
        
        Public Sub New()
            MyBase.New
            FormType = "CONFSERIE10"
            ItemUID = "7"
        End Sub

        <B1Listener(BoEventTypes.et_CLICK, False)>
        Public Overridable Sub OnAfterClick(ByVal pVal As ItemEvent)
            Dim ActionSuccess As Boolean = pVal.ActionSuccess
            Dim form As Form = B1Connections.theAppl.Forms.Item(pVal.FormUID)
            Dim item As Item = form.Items.Item("7")
            Dim button As Button = CType(item.Specific, Button)
            Dim oMatrix As SAPbouiCOM.Matrix
            Dim cell As Cells
            'ADD YOUR ACTION CODE HERE ...

            oMatrix = form.Items.Item("22").Specific

            Dim columns As Columns = oMatrix.Columns
            form.DataSources.DBDataSources.Item("@SERIESO").Clear()
            Dim rowCnt As Integer = oMatrix.RowCount

            oMatrix.AddRow(1)
            columns = oMatrix.Columns

            cell = columns.Item("V_4").Cells
            cell.Item(oMatrix.RowCount).Click(BoCellClickType.ct_Regular)

        End Sub

    End Class
End Namespace