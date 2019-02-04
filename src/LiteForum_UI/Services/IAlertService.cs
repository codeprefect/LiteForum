using System;
using LiteForum_UI.Models;

namespace LiteForum_UI.Services {
  public interface IAlertService
  {
      void Success(string message, bool keepAfterNavChange = false);
      void Warning(string message, bool keepAfterNavChange = false);
      void Error(string message, bool keepAfterNavChange = false);
      IObservable<AlertMessage> GetAlertObservable();
  }
}
