using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShakeMe.ViewModels;

namespace ShakeMe.Views;

public partial class UserProfilePage : ContentPage
{
    public UserProfilePage(UserProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}