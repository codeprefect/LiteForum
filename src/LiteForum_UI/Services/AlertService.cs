using Microsoft.AspNetCore.Blazor.Routing;
using Microsoft.AspNetCore.Blazor.Services;
using LiteForum_UI.Models;
using System;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace LiteForum_UI.Services
{
    public class AlertService : IAlertService, IDisposable
    {
        protected IUriHelper _uriHelper { get; }
        private Subject<AlertMessage> AlertReceived = new Subject<AlertMessage>();

        public AlertService(IUriHelper uriHelper)
        {
            this._uriHelper = uriHelper;
            this._uriHelper.OnLocationChanged += this.OnLocationChanges;
        }

        public void OnLocationChanges(object sender, string location) => this.AlertReceived.OnNext(null); // trigger to remove stale alerts

        public void Success(string message, bool keepAfterNavChange = false) =>
            this.AlertReceived.OnNext(new AlertMessage(AlertType.Success, message, keepAfterNavChange));

        public void Warning(string message, bool keepAfterNavChange = false) =>
            this.AlertReceived.OnNext(new AlertMessage(AlertType.Warning, message, keepAfterNavChange));

        public void Error(string message, bool keepAfterNavChange = false) =>
            this.AlertReceived.OnNext(new AlertMessage(AlertType.Error, message, keepAfterNavChange));

        public void Dispose()
        {
            this._uriHelper.OnLocationChanged -= this.OnLocationChanges;
        }

        public IObservable<AlertMessage> GetAlertObservable() => this.AlertReceived.AsObservable();
    }
}
