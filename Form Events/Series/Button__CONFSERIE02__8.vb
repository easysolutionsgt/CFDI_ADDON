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
    
    Public Class Button__CONFSERIE02__8
        Inherits B1Item
        
        Public Sub New()
            MyBase.New
            FormType = "CONFSERIE10"
            ItemUID = "8"
        End Sub

        <B1Listener(BoEventTypes.et_CLICK, False)>
        Public Overridable Sub OnAfterClick(ByVal pVal As ItemEvent)
            Dim ActionSuccess As Boolean = pVal.ActionSuccess
            Dim form As Form = B1Connections.theAppl.Forms.Item(pVal.FormUID)
            Dim item As Item = form.Items.Item("8")
            Dim button As Button = CType(item.Specific, Button)

            Dim iSelectedRow As Integer
            Dim dSacosPendientes As Integer = 0
            Dim dCantidadLinea As Integer = 0
            'ADD YOUR ACTION CODE HERE ...
            Dim matrix As Matrix

            matrix = form.Items.Item("22").Specific


            Try
                iSelectedRow = Matrix.GetNextSelectedRow(0, BoOrderType.ot_SelectionOrder)

                Matrix.FlushToDataSource()
                '<-Flush all data to datasource
                form.DataSources.DBDataSources.Item("@SERIESO").RemoveRecord(iSelectedRow - 1)
                ''<-Delete from datasource
                matrix.LoadFromDataSource()
                '<-Load data to matrix


                form.Mode = BoFormMode.fm_UPDATE_MODE
            Catch ex As Exception

            End Try


        End Sub

    End Class
End Namespace
