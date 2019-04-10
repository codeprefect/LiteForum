using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Services;
using LiteForum_UI.Models;
using System;

namespace LiteForum_UI.Services
{
    public class AlertService : IAlertService, IDisposable
    {
        protected IUriHelper _uriHelper { get; }
        protected EventHandler<AlertMessage> AlertReceived { get; set; }

        public AlertService(IUriHelper uriHelper)
        {
            _uriHelper = uriHelper;
            _uriHelper.OnLocationChanged += OnLocationChanges;
        }

        public void OnLocationChanges(object sender, string location) => AlertReceived.Invoke(this, null); // trigger to remove stale alerts

        public void Success(string message, bool keepAfterNavChange = false) =>
            AlertReceived.Invoke(this, new AlertMessage(AlertType.Success, message, keepAfterNavChange));

        public void Warning(string message, bool keepAfterNavChange = false) =>
            AlertReceived.Invoke(this, new AlertMessage(AlertType.Warning, message, keepAfterNavChange));

        public void Error(string message, bool keepAfterNavChange = false) =>
            AlertReceived.Invoke(this, new AlertMessage(AlertType.Error, message, keepAfterNavChange));

        public void OnAlertReceived(AlertMessage alert) {
            var handler = AlertReceived;
            if(handler != null) handler(this, alert);
        }

        public void Dispose()
        {
            _uriHelper.OnLocationChanged -= OnLocationChanges;
        }

        public void AddAlertReceivedHandler(EventHandler<AlertMessage> handler) => AlertReceived += handler;
        public void RemoveAlertReceivedHandler(EventHandler<AlertMessage> handler) => AlertReceived -= handler;
    }
}
