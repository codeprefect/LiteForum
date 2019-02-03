using Microsoft.AspNetCore.Blazor.Routing;
using Microsoft.AspNetCore.Blazor.Services;
using LiteForum_UI.Models;
using System;

namespace LiteForum_UI.Services
{
    public class AlertService : IDisposable
    {
        public IUriHelper _uriHelper { get; }

        public event EventHandler<AlertMessage> AlertReceived;

        public AlertService(IUriHelper uriHelper)
        {
            _uriHelper = uriHelper;
            _uriHelper.OnLocationChanged += this.OnLocationChanges;
        }

        public void OnLocationChanges(object sender, string location) => this.OnAlertReceived(); // trigger to remove stale alerts

        protected virtual void OnAlertReceived(AlertMessage e = null) {
            EventHandler<AlertMessage> handle = this.AlertReceived;
            if (handle != null) handle(this, e);
        }

        public void Success(string message, bool keepAfterNavChange = false) =>
            this.OnAlertReceived(new AlertMessage(AlertType.Success, message, keepAfterNavChange));

        public void Warning(string message, bool keepAfterNavChange = false) =>
            this.OnAlertReceived(new AlertMessage(AlertType.Warning, message, keepAfterNavChange));

        public void Error(string message, bool keepAfterNavChange = false) =>
            this.OnAlertReceived(new AlertMessage(AlertType.Error, message, keepAfterNavChange));

        public void Dispose()
        {
            _uriHelper.OnLocationChanged -= this.OnLocationChanges;
        }
    }
}
