angular.module("umbraco").config(function ($provide) {
  $provide.decorator("umbPropertyEditorDirective", function ($delegate, $http, $compile) {
    var directive = $delegate[0];
    var linkFn = directive.link;

    directive.compile = function () {
      return function (scope, element) {
        if (scope.model && (scope.model.view === "textbox" || scope.model.view === "textarea")) {
          // Determine API URL based on view type
          var apiUrl = scope.model.view === "textbox"
            ? "/umbraco/backoffice/api/Genie/IsGenieTextBoxEnabled"
            : "/umbraco/backoffice/api/Genie/IsGenieTextAreaEnabled";

          $http.get(apiUrl).then((response) => {
            var isEnabled = response.data;
            if (!isEnabled) {
              return; // Skip if disabled
            }

            setTimeout(() => {
              var input = element.find("input, textarea");

              // Helper: Create and show a loader after the input.
              function showLoader() {
                var loader = document.createElement("div");
                loader.className = "loader-custom";
                loader.textContent = "Loading...";
                // Simple inline style for demo purposes.
                loader.style.position = "absolute";
                loader.style.background = "rgba(0,0,0,0.3)";
                loader.style.color = "#fff";
                loader.style.padding = "5px 10px";
                loader.style.borderRadius = "4px";
                // Position loader relative to input (adjust as needed)
                var pos = input.offset();
                loader.style.top = pos.top - 30 + "px";
                loader.style.left = pos.left + "px";
                document.body.appendChild(loader);
                return loader;
              }

              // Define a shared style for shadow DOM elements.
              const shadowButtonStyle = `
                              <style>
                                .genie-btn {
                                  display: inline-block;
                                  background-color: #1f254e;
                                  color: #fff;
                                  padding: 8px 14px;
                                  border: none;
                                  border-radius: 4px;
                                  cursor: pointer;
                                  font-size: 14px;
                                  transition: background-color 0.3s ease;
                                  margin: 5px;
                                }
                                .genie-btn:hover {
                                  background-color: #585757;
                                }
                                /* Remove left margin for the first button using :first-of-type */
                                .genie-btn:first-of-type {
                                  margin-left: 0;
                                }
                                /* Remove any margin from inputs and text fields */
                                input, .tox-textfield {
                                  margin: 0 !important;
                                }
                                /* Popup styling */
                                .tox-dialog-wrap {
                                position: fixed;
                                top: 0;
                                left: 0;
                                width: 100%;
                                height: 100%;
                                display: flex;
                                justify-content: center;
                                align-items: center;
                                background: rgba(255, 255, 255, 0.75);
                                z-index: 11000;
                                }

                                .tox-dialog {
                                background: #fff;
                                border-radius: 8px;
                                box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
                                width: 450px;
                                max-width: 90%;
                                position: relative;
                                animation: dialogFadeIn 0.2s ease-out;
                                overflow: hidden;
                                }

                                @keyframes dialogFadeIn {
                                from { opacity: 0; transform: translateY(-10px); }
                                to { opacity: 1; transform: translateY(0); }
                                }

                                .tox-dialog__header {
                                display: flex;
                                justify-content: space-between;
                                align-items: center;
                                padding: 12px 20px;
                                border-bottom: none;
                                }

                                .tox-dialog__title {
                                font-size: 18px;
                                font-weight: 500;
                                color: #333;
                                }

                                .tox-dialog__content-js {
                                padding: 0 20px 10px;
                                }

                                .tox-dialog__footer {
                                display: flex;
                                justify-content: flex-end;
                                gap: 10px;
                                padding: 10px 20px;
                                border-top: none;
                                background-color: #f9f9f9;
                                }

                                /* Close button in header - explicitly set color to override inheritance */
                                .tox-button--icon {
                                display: flex;
                                align-items: center;
                                justify-content: center;
                                background: transparent;
                                border: none;
                                padding: 8px;
                                cursor: pointer;
                                color: #666 !important; /* Force override any inherited color */
                                width: 32px;
                                height: 32px;
                                border-radius: 4px;
                                background-color: transparent !important; /* Force transparent background */
                                }

                                .tox-button--icon:hover {
                                background-color: #f0f0f0 !important;
                                color: #333 !important;
                                }

                                .tox-button--icon .tox-icon {
                                color: #666;
                                }

                                .tox-button--icon:hover .tox-icon {
                                color: #333;
                                }

                                /* Button styles - separate from close button */
                                .tox-button:not(.tox-button--icon) {
                                padding: 8px 16px;
                                border-radius: 4px;
                                font-size: 14px;
                                font-weight: 500;
                                cursor: pointer;
                                border: none;
                                transition: all 0.2s;
                                }

                                .tox-button--secondary {
                                background-color: #f0f0f0;
                                color: #333;
                                }

                                .tox-button--secondary:hover {
                                background-color: #e0e0e0;
                                }

                                /* Primary action button */
                                .tox-button:not(.tox-button--secondary):not(.tox-button--icon) {
                                background-color: #4476f6;
                                color: white;
                                }

                                .tox-button:not(.tox-button--secondary):not(.tox-button--icon):hover {
                                background-color: #3a65d8;
                                }

                                /* Form elements */
                                .tox-label {
                                display: block;
                                margin-bottom: 4px;
                                font-size: 14px;
                                color: #666;
                                }

                                .tox-textfield {
                                width: 100%;
                                box-sizing: border-box;
                                padding: 8px 12px;
                                border: 1px solid #ddd;
                                border-radius: 4px;
                                font-size: 14px;
                                transition: border 0.2s;
                                }

                                .tox-textfield:focus {
                                outline: none;
                                border-color: #4476f6;
                                }

                                .tox-form__group {
                                margin-bottom: 10px;
                                width: 100%;
                                }
                              </style>
                            `;

              // Helper: Create a shadow host and inject provided HTML
              function createShadowContent(html) {
                var host = document.createElement("div");
                var shadow = host.attachShadow({ mode: "open" });
                shadow.innerHTML = shadowButtonStyle + html;
                return { host, shadow };
              }

              // Updated simple popup using shadow DOM with new markup.
              function showSimplePopup(callback) {
                var { host, shadow } = createShadowContent(`
                    <div tabindex="-1" class="tox-dialog-wrap">
                        <div class="tox-dialog-wrap__backdrop"></div>
                        <div role="dialog" aria-modal="true" tabindex="-1" class="tox-dialog" aria-labelledby="dialog-label">
                        <div role="presentation" class="tox-dialog__header">
                            <div class="tox-dialog__title" id="dialog-label">Generate/Edit Text with AI</div>
                            <button type="button" aria-label="Close" title="Close" class="tox-button tox-button--icon close-dialog">
                            <span class="tox-icon"><svg width="24" height="24" focusable="false"><path d="M17.3 8.2 13.4 12l3.9 3.8a1 1 0 0 1-1.5 1.5L12 13.4l-3.8 3.9a1 1 0 0 1-1.5-1.5l3.9-3.8-3.9-3.8a1 1 0 0 1 1.5-1.5l3.8 3.9 3.8-3.9a1 1 0 0 1 1.5 1.5Z" fill-rule="evenodd"></path></svg></span>
                            </button>
                        </div>
                        <div class="tox-dialog__content-js">
                            <div class="tox-form">
                            <div class="tox-form__group">
                                <label class="tox-label" for="form-field">Enter prompt</label>
                                <input type="text" class="tox-textfield" id="form-field">
                            </div>
                            </div>
                        </div>
                        <div class="tox-dialog__footer">
                            <button title="Close" type="button" class="tox-button tox-button--secondary close-dialog">Close</button>
                            <button title="Generate" type="button" class="tox-button generate-dialog">Generate</button>
                        </div>
                        </div>
                    </div>
                    `);
                document.body.appendChild(host);

                // Close dialog when clicking any close button.
                shadow.querySelectorAll(".close-dialog").forEach((btn) => {
                  btn.addEventListener("click", function (e) {
                    host.remove();
                  });
                });

                // Generate button: capture input and callback.
                shadow
                  .querySelector(".generate-dialog")
                  .addEventListener("click", function (e) {
                    var result =
                      shadow.querySelector(".tox-textfield").value;
                    host.remove();
                    callback(result);
                  });

                // Dismiss on ESC key.
                function escHandler(e) {
                  if (e.keyCode === 27) {
                    host.remove();
                    document.removeEventListener("keydown", escHandler);
                  }
                }
                document.addEventListener("keydown", escHandler);
              }

              if (scope.model.view === "textbox") {
                var { host, shadow } = createShadowContent(`
                                  <button type="button" class="genie-btn btn-paraphrase">paraphrase</button>
                                `);
                var paraphraseBtn = shadow.querySelector(".btn-paraphrase");
                paraphraseBtn.addEventListener("click", function (e) {
                  e.preventDefault();
                  paraphraseBtn.disabled = true;
                  var origText = paraphraseBtn.textContent;
                  var origBg = paraphraseBtn.style.backgroundColor;
                  paraphraseBtn.textContent = "Generating";
                  paraphraseBtn.style.backgroundColor = "#585757";
                  var content = input.val();
                  var loader = showLoader();
                  // Updated endpoint for textbox paraphrase
                  fetch("/umbraco/api/AIGeneration/ParaphraseNormal", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ prompt: content }),
                  })
                    .then((response) => response.json())
                    .then((data) => {
                      input.val(data.text);
                      input.trigger("input");
                    })
                    .catch((error) => {
                      console.error("Error paraphrasing content:", error);
                    })
                    .finally(() => {
                      loader.remove();
                      paraphraseBtn.disabled = false;
                      paraphraseBtn.textContent = origText;
                      paraphraseBtn.style.backgroundColor = origBg;
                    });
                });
                input.after(host);
              } else if (scope.model.view === "textarea") {
                // Create shadow host for three buttons.
                var { host, shadow } = createShadowContent(`
                                  <button type="button" class="genie-btn btn-generate">Generate</button>
                                  <button type="button" class="genie-btn btn-edit-generated">Edit Generated</button>
                                  <button type="button" class="genie-btn btn-paraphrase">paraphrase</button>
                                `);
                // For "Generate": show popup, then call the Generate API with the popup input.
                var genBtn = shadow.querySelector(".btn-generate");
                genBtn.addEventListener("click", function (e) {
                  e.preventDefault();
                  showSimplePopup(function (userInput) {
                    genBtn.disabled = true;
                    var origText = genBtn.textContent;
                    var origBg = genBtn.style.backgroundColor;
                    genBtn.textContent = "Generating";
                    genBtn.style.backgroundColor = "#585757";
                    var loader = showLoader();
                    fetch("/umbraco/api/AIGeneration/GenerateNormal", {
                      method: "POST",
                      headers: { "Content-Type": "application/json" },
                      body: JSON.stringify({ prompt: userInput }),
                    })
                      .then((response) => response.json())
                      .then((data) => {
                        input.val(data.text);
                        input.trigger("input");
                      })
                      .catch((error) => {
                        console.error("Error generating text:", error);
                      })
                      .finally(() => {
                        loader.remove();
                        genBtn.disabled = false;
                        genBtn.textContent = origText;
                        genBtn.style.backgroundColor = origBg;
                      });
                  });
                });
                // For "Edit Generated": show popup, then call the Edit Generated API with the original content and popup input.
                var editBtn = shadow.querySelector(".btn-edit-generated");
                editBtn.addEventListener("click", function (e) {
                  e.preventDefault();
                  var content = input.val();
                  showSimplePopup(function (userInput) {
                    editBtn.disabled = true;
                    var origText = editBtn.textContent;
                    var origBg = editBtn.style.backgroundColor;
                    editBtn.textContent = "Generating";
                    editBtn.style.backgroundColor = "#585757";
                    var loader = showLoader();
                    fetch(
                      "/umbraco/api/AIGeneration/EditGeneratedTextNormal",
                      {
                        method: "POST",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify({
                          prompt: content,
                          editedText: userInput,
                        }),
                      }
                    )
                      .then((response) => response.json())
                      .then((data) => {
                        input.val(data.text);
                        input.trigger("input");
                      })
                      .catch((error) => {
                        console.error("Error sending request:", error);
                      })
                      .finally(() => {
                        loader.remove();
                        editBtn.disabled = false;
                        editBtn.textContent = origText;
                        editBtn.style.backgroundColor = origBg;
                      });
                  });
                });
                // Paraphrase remains calling its API on popup click as before.
                var paraBtn = shadow.querySelector(".btn-paraphrase");
                paraBtn.addEventListener("click", function (e) {
                  e.preventDefault();
                  paraBtn.disabled = true;
                  var origText = paraBtn.textContent;
                  var origBg = paraBtn.style.backgroundColor;
                  paraBtn.textContent = "Generating";
                  paraBtn.style.backgroundColor = "#585757";
                  var content = input.val();
                  var loader = showLoader();
                  fetch("/umbraco/api/AIGeneration/ParaphraseNormal", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ prompt: content }),
                  })
                    .then((response) => response.json())
                    .then((data) => {
                      input.val(data.text);
                      input.trigger("input");
                    })
                    .catch((error) => {
                      console.error("Error paraphrasing content:", error);
                    })
                    .finally(() => {
                      loader.remove();
                      paraBtn.disabled = false;
                      paraBtn.textContent = origText;
                      paraBtn.style.backgroundColor = origBg;
                    });
                });
                input.after(host);
              }
            }, 500);
          });
        }
        if (linkFn) linkFn.apply(this, arguments);
      };
    };

    return $delegate;
  });
});
