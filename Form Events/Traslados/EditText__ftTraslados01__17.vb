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
    
    Public Class EditText__ftTraslados01__17
        Inherits B1Item
        
        Public Sub New()
            MyBase.New
            FormType = "ftTraslados01"
            ItemUID = "17"
        End Sub

        <B1Listener(BoEventTypes.et_CHOOSE_FROM_LIST, False)>
        Public Overridable Sub OnAfterChooseFromList(ByVal pVal As ItemEvent)
            Dim ActionSuccess As Boolean = pVal.ActionSuccess
            Dim form As Form = B1Connections.theAppl.Forms.Item(pVal.FormUID)
            Dim item As Item = form.Items.Item("17")
            Dim edittext As EditText = CType(item.Specific, EditText)
            'ADD YOUR ACTION CODE HERE ...


            Dim oDataTable As SAPbouiCOM.DataTable

            If pVal.BeforeAction = False Then
                oDataTable = pVal.SelectedObjects

                Try

                    form.Freeze(True)

                    If oDataTable Is Nothing Then
                        Exit Try
                    End If

                    ' form.Items.Item("19").Click()
                    form.Items.Item("17").Specific.Value = oDataTable.GetValue(0, 0)


                Catch ex As Exception
                Finally
                    form.Freeze(False)
                End Try
            End If




        End Sub

    End Class
End Namespace
