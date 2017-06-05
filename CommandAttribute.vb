﻿<AttributeUsage(AttributeTargets.Method)>
Public Class CommandAttribute
    Inherits Attribute

#Region "Method Property"
    Private _Method As Reflection.MethodInfo

    Public Property Method As Reflection.MethodInfo
        Get
            Return Me._Method
        End Get
        Set(ByVal Value As Reflection.MethodInfo)
            If Me._Method IsNot Nothing Then
                Throw New InvalidOperationException($"Cannot set {NameOf(Me.Method)} twice.")
            End If
            If Value Is Nothing Then
                Throw New ArgumentNullException(NameOf(Value))
            End If
            Me._Method = Value
        End Set
    End Property
#End Region

#Region "Description Property"
    Private _Description As String

    Public Property Description As String
        Get
            Return Me._Description
        End Get
        Set(ByVal Value As String)
            If Me._Description IsNot Nothing Then
                Throw New InvalidOperationException($"Cannot set {NameOf(Me.Description)} twice.")
            End If
            If Value Is Nothing Then
                Throw New ArgumentNullException(NameOf(Value))
            End If
            Me._Description = Value
        End Set
    End Property
#End Region

#Region "ParametersDescriptions Property"
    Private _ParametersDescriptions As IReadOnlyList(Of ParameterDescriptionAttribute)

    Public ReadOnly Property ParametersDescriptions As IReadOnlyList(Of ParameterDescriptionAttribute)
        Get
            Return Me._ParametersDescriptions
        End Get
    End Property

    Public Sub SetParametersDescriptions(ByVal Value As IEnumerable(Of ParameterDescriptionAttribute))
        If Me._ParametersDescriptions IsNot Nothing Then
            Throw New InvalidOperationException($"Cannot set {NameOf(Me.ParametersDescriptions)} twice.")
        End If
        If Value Is Nothing Then
            Throw New ArgumentNullException(NameOf(Value))
        End If
        Me._ParametersDescriptions = New ObjectModel.ReadOnlyCollection(Of ParameterDescriptionAttribute)(Value.ToArray())
    End Sub
#End Region

End Class
