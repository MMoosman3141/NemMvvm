using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NemMvvm {
  /// <summary>
  /// Provides a common interface to expose the RaiseCanExecuteChanged method.
  /// </summary>
  public interface IFoundationCommand : ICommand {
    /// <summary>
    /// Raises the CanExecuteChanged event
    /// </summary>
    void RaiseCanExecuteChanged();
  }
}
