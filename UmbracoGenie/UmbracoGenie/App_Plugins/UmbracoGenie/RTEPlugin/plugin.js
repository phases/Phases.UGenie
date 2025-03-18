'use strict';
(function () {
    function plugin(tinymce) {
        tinymce.PluginManager.add('aiTextGenerator', function (editor) {
            let currentSuggestion = "";
            let suggestions = [];
            let isGenerating = false;

            function debounce(func, wait) {
                let timeout;
                return function () {
                    const context = this, args = arguments;
                    clearTimeout(timeout);
                    timeout = setTimeout(() => func.apply(context, args), wait);
                };
            }

            const onInputChange = debounce(function () {
                const content = editor.getContent({ format: 'text' });
                const cursorPosition = editor.selection.getRng().startOffset;
                const textBeforeCursor = content.slice(0, cursorPosition);
                const lastWordMatch = textBeforeCursor.match(/\b\w+$/);

                if (lastWordMatch) {
                    getSuggestions(lastWordMatch[0]);
                } else {
                    suggestions = [];
                    currentSuggestion = "";
                }
            }, 300);

            function showNotification(message, type) {
                const notification = editor.notificationManager.open({
                    text: message,
                    type: type,
                    timeout: 2000 // Auto-hide after 2 seconds
                });
            }
            function getSuggestions(term) {
                // Replace this with your actual API call
                fetch('/umbraco/api/AIGeneration/GetSuggestions', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ prompt: term })
                })
                    .then(response => response.json())
                    .then(data => {
                        suggestions = data;
                        if (suggestions.length > 0) {
                            currentSuggestion = " " + suggestions[0].toLowerCase();
                            showSuggestion();
                        } else {
                            currentSuggestion = "";
                            hideSuggestion();
                        }
                    })
                    .catch(error => {
                        console.error('Error getting suggestions:', error);
                        suggestions = [];
                        currentSuggestion = "";
                        hideSuggestion();
                    });
            }

            function showSuggestion() {
                const suggestionSpan = editor.getContainer().querySelector('.mce-suggestion');
                if (suggestionSpan) {
                    suggestionSpan.textContent = currentSuggestion;
                    suggestionSpan.style.display = 'inline';
                }
            }

            function hideSuggestion() {
                const suggestionSpan = editor.getContainer().querySelector('.mce-suggestion');
                if (suggestionSpan) {
                    suggestionSpan.style.display = 'none';
                }
            }

            function applySuggestion() {
                if (currentSuggestion) {
                    editor.insertContent(currentSuggestion.trim());
                    currentSuggestion = "";
                    hideSuggestion();
                }
            }

            function openGeneratePopup() {
                editor.windowManager.open({
                    title: 'Generate Text with AI',
                    body: {
                        type: 'panel',
                        items: [
                            {
                                type: 'input',
                                name: 'generatePrompt',
                                label: 'Enter prompt'
                            }
                        ]
                    },
                    buttons: [
                        {
                            type: 'cancel',
                            text: 'Close'
                        },
                        {
                            type: 'submit',
                            text: 'Generate',
                            primary: true
                        }
                    ],
                    onSubmit: function (api) {
                        isGenerating = true;
                        editor.setProgressState(true); // Show loading state
                        const data = api.getData();
                        const generatePrompt = data.generatePrompt;

                        // Replace this with your actual API call
                        fetch('/umbraco/api/AIGeneration/Generate', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify({ prompt: generatePrompt })
                        })
                            .then(response => response.json())
                            .then(data => {
                                editor.setContent(data.text); // Replace all content
                                showNotification('Text generated successfully!', 'success');
                            })
                            .catch(error => {
                                console.error('Error generating text:', error);
                                showNotification('Error generating text. Please try again.', 'error');
                            })
                            .finally(() => {
                                isGenerating = false;
                                editor.setProgressState(false); // Hide loading state
                            });

                        api.close();
                    }
                });
            }

            function openCustomPopup() {
                editor.windowManager.open({
                    title: 'Edit content with AI',
                    body: {
                        type: 'panel',
                        items: [
                            {
                                type: 'input',
                                name: 'customText',
                                label: 'Enter the changes'
                            }
                        ]
                    },
                    buttons: [
                        {
                            type: 'cancel',
                            text: 'Close'
                        },
                        {
                            type: 'submit',
                            text: 'Generate',
                            primary: true
                        }
                    ],
                    onSubmit: function (api) {
                        isGenerating = true;
                        editor.setProgressState(true);
                        const data = api.getData();
                        const customText = data.customText;
                        const content = editor.getContent({ format: 'text' });
                        // Replace this with your actual API call
                        fetch('/umbraco/api/AIGeneration/EditGeneratedText', {
                            method: 'POST',
                            headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify({ prompt: content, editedText: customText })
                        })
                            .then(response => response.json())
                            .then(data => {
                                editor.setContent(data.text); // Replace all content with refined HTML content
                                showNotification('Text refined successfully!', 'success');
                            })
                            .catch(error => {
                                console.error('Error sending request:', error);
                                showNotification('Error sending request. Please try again.', 'error');
                            }).finally(() => {
                                isGenerating = false;
                                editor.setProgressState(false); // Hide loading state
                            });

                        api.close();
                    }
                });
            }

            function paraphraseContent() {
                isGenerating = true;
                const content = editor.getContent({ format: 'text' });

                editor.setProgressState(true); // Show loading state

                // Replace this with your actual API call
                fetch('/umbraco/api/AIGeneration/Paraphrase', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ prompt: content })
                })
                    .then(response => response.json())
                    .then(data => {
                        editor.setContent(data.text); // Replace all content
                        showNotification('Content paraphrased successfully!', 'success');
                    })
                    .catch(error => {
                        console.error('Error paraphrasing content:', error);
                        showNotification('Error paraphrasing content. Please try again.', 'error');
                    })
                    .finally(() => {
                        isGenerating = false;
                        editor.setProgressState(false); // Hide loading state
                    });
            }

            
            editor.on('init', function () {
                const editorContainer = editor.getContainer();
                const suggestionSpan = document.createElement('span');
                suggestionSpan.className = 'mce-suggestion';
                suggestionSpan.style.opacity = '0.5';
                suggestionSpan.style.position = 'absolute';
                suggestionSpan.style.pointerEvents = 'none';
                editorContainer.appendChild(suggestionSpan);
            });

            editor.on('keyup', onInputChange);

            editor.on('keydown', function (e) {
                if (e.keyCode === 9 && currentSuggestion) { // Tab key
                    e.preventDefault();
                    applySuggestion();
                }
            })

            editor.ui.registry.addIcon('custom-generate-icon', '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path fill-rule="evenodd" clip-rule="evenodd" d="m15.061 9.68 1.415 1.414 1.403-1.403-1.415-1.414zm1.404-3.525L20 9.691 8.686 21.005l-3.535-3.536zM11.364 5.06 9.596 6.829l-1.06-1.06L10.303 4zM6.768 6.829 5 5.061 6.06 4l1.768 1.768zm3.535 3.536L8.536 8.596l1.06-1.06 1.768 1.767zM7.828 8.596l-1.767 1.768L5 9.304l1.768-1.768z" fill="#1F2328"/></svg>');
            editor.ui.registry.addIcon('custom-paraphrase-icon', '<svg width="24px" height="24px" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <path d="M10 10V11C10 11.9319 10 12.3978 9.84776 12.7654C9.64477 13.2554 9.25542 13.6448 8.76537 13.8478C8.39782 14 7.93188 14 7 14H6.5C5.09554 14 4.39331 14 3.88886 13.6629C3.67048 13.517 3.48298 13.3295 3.33706 13.1111C3 12.6067 3 11.9045 3 10.5V9.65685C3 8.83935 3 8.4306 3.15224 8.06306C3.30448 7.69552 3.59351 7.40649 4.17157 6.82843L4.76035 6.23965C5.21781 5.78219 6 6.10618 6 6.75313V7.12132C6 7.6066 6.3934 8 6.87868 8H8C9.10457 8 10 8.89543 10 10Z" stroke="#323232" stroke-width="2" stroke-linejoin="round"></path> <path d="M21 16V17C21 17.9319 21 18.3978 20.8478 18.7654C20.6448 19.2554 20.2554 19.6448 19.7654 19.8478C19.3978 20 18.9319 20 18 20H17.5C16.0955 20 15.3933 20 14.8889 19.6629C14.6705 19.517 14.483 19.3295 14.3371 19.1111C14 18.6067 14 17.9045 14 16.5V15.6569C14 14.8394 14 14.4306 14.1522 14.0631C14.3045 13.6955 14.5935 13.4065 15.1716 12.8284L15.7604 12.2396C16.2178 11.7822 17 12.1062 17 12.7531V13.1213C17 13.6066 17.3934 14 17.8787 14H19C20.1046 14 21 14.8954 21 16Z" stroke="#323232" stroke-width="2" stroke-linejoin="round"></path> </g></svg>');
            editor.ui.registry.addIcon('custom-edit-icon', '<svg width="24px" height="24px" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <path d="M4 5L15 5" stroke="#000000" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></path> <path d="M4 8H15" stroke="#000000" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></path> <path d="M4 11H11" stroke="#000000" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></path> <path d="M18.4563 13.5423L13.9268 18.0719C13.6476 18.3511 13.292 18.5414 12.9048 18.6188L10.8153 19.0367L11.2332 16.9472C11.3106 16.5601 11.5009 16.2045 11.7801 15.9253L16.3096 11.3957M18.4563 13.5423L19.585 12.4135C19.9755 12.023 19.9755 11.3898 19.585 10.9993L18.8526 10.2669C18.4621 9.8764 17.8289 9.8764 17.4384 10.2669L16.3096 11.3957M18.4563 13.5423L16.3096 11.3957" stroke="#000000" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></path> </g></svg>');


            editor.ui.registry.addButton('generateText', {
                text: 'AI Generate Text',
                icon: 'custom-generate-icon',
                onAction: function () {
                    openGeneratePopup();
                }
            });

            editor.ui.registry.addButton('openCustomPopup', {
                text: 'AI Edit Text',
                icon: 'custom-edit-icon',
                onAction: function () {
                    openCustomPopup();
                }
            });


            editor.ui.registry.addButton('paraphraseContent', {
                text: 'AI Paraphrase',
                icon: 'custom-paraphrase-icon',
                onAction: function () {
                    paraphraseContent()
                }
            })

            return {
                getMetadata: function () {
                    return {
                        name: 'AI Text Generator',
                        url: 'http://example.com/docs/aiTextGenerator'
                    };
                }
            };
        });
    }

    if (window && 'tinymce' in window) {
        plugin(window.tinymce)
    }
})();