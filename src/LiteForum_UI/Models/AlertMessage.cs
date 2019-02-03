using System;
using System.Collections.Generic;

namespace LiteForum_UI.Models
{
  public class AlertMessage : EventArgs
  {
    public readonly AlertType type;

    public string message { get; }

    public AlertMessage() {}

    public AlertMessage(AlertType alertType, string message)
    {
      this.type = alertType;
      this.message = message;
    }

    public override string ToString() => $"Type: {this.type}\nMessage: {this.message}";

    public bool IsSuccess() => this.type == AlertType.Success;
    public bool IsWarning() => this.type == AlertType.Warning;
    public bool IsError() => this.type == AlertType.Error;
  }

  public enum AlertType
  {
    Error = 1,
    Warning = 2,
    Success = 3
  }
}
