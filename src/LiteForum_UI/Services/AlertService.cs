using Microsoft.AspNetCore.Blazor.Routing;
using Microsoft.AspNetCore.Blazor.Services;
using LiteForum_UI.Models;
using System;
using Microsoft.Extensions.Logging;

namespace LiteForum_UI.Services
{
    public class AlertService : IDisposable
    {
        private bool _keepAfterNavChange;
        private readonly ILogger<AlertService> _logger;

        public IUriHelper _uriHelper { get; }

        public event EventHandler<AlertMessage> AlertReceived;

        public AlertService(IUriHelper uriHelper, ILogger<AlertService> logger)
        {
            _uriHelper = uriHelper;
            _uriHelper.OnLocationChanged += this.OnLocationChanges;
            _logger = logger;
        }

        public void OnLocationChanges(object sender, string location) => RefreshAlertSubscriber(sender);

        private void RefreshAlertSubscriber(object sender) {
            _logger.LogInformation("navigation changed");
            if(this._keepAfterNavChange) {
                this._keepAfterNavChange = false;
            } else {
                this.OnAlertReceived();
            }
        }

        protected virtual void OnAlertReceived(AlertMessage e = null) {
            EventHandler<AlertMessage> handle = this.AlertReceived;
            if (handle != null) handle(this, e);
        }

        public void Success(string message, bool keepAfterNavChange = false) {
            this._keepAfterNavChange = keepAfterNavChange;
            this.OnAlertReceived(new AlertMessage(AlertType.Success, message));
        }

        public void Warning(string message, bool keepAfterNavChange = false) {
            this._keepAfterNavChange = keepAfterNavChange;
            this.OnAlertReceived(new AlertMessage(AlertType.Warning, message));
        }

        public void Error(string message, bool keepAfterNavChange = false) {
            this._keepAfterNavChange = keepAfterNavChange;
            this.OnAlertReceived(new AlertMessage(AlertType.Error, message));
        }

        public void Dispose()
        {
            _uriHelper.OnLocationChanged -= this.OnLocationChanges;
        }
    }
}
