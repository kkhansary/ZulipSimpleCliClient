<AttributeUsage(AttributeTargets.Method, AllowMultiple:=True)>
Public Class CommandAliasAttribute
    Inherits Attribute

    Public Sub New([Alias] As String)
        Me._Alias = [Alias]
    End Sub

#Region "Alias Read-Only Property"
    Private ReadOnly _Alias As String

    Public ReadOnly Property [Alias] As String
        Get
            Return Me._Alias
        End Get
    End Property
#End Region

End Class
