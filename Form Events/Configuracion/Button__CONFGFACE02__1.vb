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
    
    Public Class Button__CONFEL03__1
        Inherits B1Item
        
        Public Sub New()
            MyBase.New
            FormType = "ftFEL01"
            ItemUID = "1"
        End Sub

        <B1Listener(BoEventTypes.et_CLICK, True)>
        Public Overridable Function OnBeforeClick(ByVal pVal As ItemEvent) As Boolean
            Dim form As Form = B1Connections.theAppl.Forms.Item(pVal.FormUID)
            Dim item As Item = form.Items.Item("1")
            Dim button As Button = CType(item.Specific, Button)
            Dim oRecordSet As SAPbobsCOM.Recordset
            Dim sQuery As String = ""
            'ADD YOUR ACTION CODE HERE ...

           
            oRecordSet = Nothing
            oRecordSet = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
            sQuery = "Select * from ""@CONFEL"" "
            oRecordSet.DoQuery(sQuery)
            oRecordSet.MoveFirst()

            If oRecordSet.RecordCount > 0 Then


                If form.Mode = BoFormMode.fm_ADD_MODE Then
                    B1Connections.theAppl.SetStatusBarMessage("Error: No Se Puede Crear El Registro, Ya existe un registro creado....", BoMessageTime.bmt_Medium, True)
                    Return False
                End If

            Else
                form.Freeze(True)

                form.Items.Item("0_U_E").Visible = True
                form.Items.Item("0_U_E").Enabled = True
                form.Items.Item("0_U_E").Specific.value = "1"
                'form.Items.Item("1").Click()
                form.Items.Item("47").Click()
                form.Items.Item("0_U_E").Visible = False

                form.Freeze(False)
            End If


            Return True
        End Function


    End Class
End Namespace
