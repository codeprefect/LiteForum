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
      this.Type = alertType;
      this.Message = message;
      this.PersistOnRouteChange = persistOnRouteChange;
    }

    public override string ToString() => $"\nType: {this.Type}\nMessage: {this.Message}\nPersistOnRouteChange: {this.PersistOnRouteChange}";

    public bool IsSuccess() => this.Type == AlertType.Success;
    public bool IsWarning() => this.Type == AlertType.Warning;
    public bool IsError() => this.Type == AlertType.Error;
  }

  public enum AlertType
  {
    Error = 1,
    Warning = 2,
    Success = 3
  }
}
