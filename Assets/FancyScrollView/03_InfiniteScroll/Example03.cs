/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Linq;
using UnityEngine;

namespace FancyScrollView.Example03
{
    class Example03 : MonoBehaviour
    {
        [SerializeField] ScrollView scrollView = default;
        private string[] itemMessages = new string[] {
            "Alan: What's u gonna do on the weekend?",
            "Ken: heya, are you in Berlin?",
            "Installing LINE, WhatsUp ..",
            "Microsoft Bing: You're in! Welcome to the new Bing!",
            "Fitbit: Your Charge 5 battery level is low",
            "Altman: yeah, I will be there.",
        };

        void Start()
        {
            var items = Enumerable.Range(0, itemMessages.Length)
                .Select(i => new ItemData($"{itemMessages[i]}"))
                .ToArray();

            scrollView.UpdateData(items);
            scrollView.SelectCell(0);
        }
    }
}
