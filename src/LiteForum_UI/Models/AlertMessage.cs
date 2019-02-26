using System;
using System.Collections.Generic;

namespace LiteForum_UI.Models
{
  public class AlertMessage : EventArgs
  {
    public readonly AlertType Type;

    public readonly string Message;

    public readonly bool PersistOnRouteChange;

    public AlertMessage() {}

    public AlertMessage(AlertType alertType, string message, bool persistOnRouteChange = false)
    {
      Type = alertType;
      Message = message;
      PersistOnRouteChange = persistOnRouteChange;
    }

    public override string ToString() => $"\nType: {Type}\nMessage: {Message}\nPersistOnRouteChange: {PersistOnRouteChange}";

    public bool IsSuccess() => Type == AlertType.Success;
    public bool IsWarning() => Type == AlertType.Warning;
    public bool IsError() => Type == AlertType.Error;
  }

  public enum AlertType
  {
    Error = 1,
    Warning = 2,
    Success = 3
  }
}
