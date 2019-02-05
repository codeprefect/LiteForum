using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Components;
using LiteForum_UI.Services;
using LiteForum_UI.Models;
using System;

namespace LiteForum_UI.Shared {
  public class AlertBase : BlazorComponent, IDisposable {
    private IDisposable alertObservable;
    public List<AlertMessage> alerts;

    [Inject]
    protected IAlertService alertService { get; set; }
    protected override void OnInit()
    {
      alertObservable = this.alertService.GetAlertObservable().Subscribe(this.UpdateAlerts);
      this.alerts = new List<AlertMessage>();
    }

    public void UpdateAlerts(AlertMessage e) {
      if(e != null) {
        this.alerts.Add(e);
        this.RemoveStaleAlert(e);
      } else {
          // route has changed removed non persisted alerts
        this.alerts.RemoveAll(t => !t.PersistOnRouteChange);
      }
      this.StateHasChanged(); // triggers update of component props
    }

    public void RemoveAlert(AlertMessage alert) => alerts.Remove(alert);

    public void Dispose() {
      alertObservable.Dispose();
    }

    public void RemoveStaleAlert(AlertMessage alert) {
      int persistInMinutes = alert.IsSuccess() ? 1
        : (alert.IsWarning() ? 3
        : (alert.IsError() ? 10 : 0));
      Task.Delay(persistInMinutes * 1000 * 60).ContinueWith(_ => {
        this.RemoveAlert(alert);
        this.StateHasChanged(); // triggers update of component props
      });
    }
  }
}
