namespace Phases.UmbracoGenie
{
    public static class SystemPrompts
    {
      #region RTE Prompts
        public static readonly string GenerateTextPrompt = @"Instructions:
1. Use the following HTML structure:
   - Wrap each main section title in <h2></h2> tags
   - Wrap each paragraph in <p></p> tags
   - Replace *italic* or *italic* with <em>italic</em>
   - Replace **bold** or **bold** with <strong>bold</strong>
2. Maintain the overall structure and emphasis of the original content.
3. Provide only the refined HTML content, without any explanations or comments.
4. Ensure all HTML tags are properly closed.
5. Do not use any inline styles or class attributes.
6. Do not use any markdown formatting in the final output.
7. Dont include ```html or ``` llm tags.

Output the refined HTML content directly, starting with the first HTML tag:";

        public static readonly string EditTextPrompt = @"Instructions:
1. Combine the original text and user edits to create an improved version.
2. Use the following HTML structure:
   - Wrap each main section title in <h2></h2> tags
   - Wrap each paragraph in <p></p> tags
   - Replace *italic* or _italic_ with <em>italic</em>
   - Replace **bold** or __bold__ with <strong>bold</strong>
3. Maintain the overall structure and emphasis of the original content.
4. Provide only the refined HTML content, without any explanations or comments.
5. Ensure all HTML tags are properly closed.
6. Do not use any inline styles or class attributes.
7. Do not use any markdown formatting in the final output.
8. Dont include ```html or ``` llm tags.

Output the refined HTML content directly, starting with the first HTML tag:";

        public static readonly string ParaphrasePrompt = @"Instructions:
1. Improve the wording while maintaining the original meaning and tone.
2. Use the following HTML structure:
   - Wrap each main section title in <h2></h2> tags
   - Wrap each paragraph in <p></p> tags
   - Replace *italic* or _italic_ with <em>italic</em>
   - Replace **bold** or __bold__ with <strong>bold</strong>
3. Maintain the overall structure and emphasis of the original content.
4. Provide only the improved HTML content, without any explanations or comments.
5. Ensure all HTML tags are properly closed.
6. Do not use any inline styles or class attributes.
7. Do not use any markdown formatting in the final output.
8. Dont include ```html or ``` llm tags.

Output the paraphrased HTML content directly, starting with the first HTML tag:";

#endregion

#region Normal Prompts

  public static readonly string GenerateTextPromptNormal = @"Instructions:
1. Improve the clarity and flow of the text while preserving the original meaning and tone.
2. Correct any grammatical errors or awkward phrasing.
3. Ensure the content is concise and easy to understand.
4. Do not add any additional information, code, or characters that were not in the original text.
5. Provide only the refined text. No explanations, comments, or extra characters.

";

  public static readonly string EditTextPromptNormal = @"Instructions:
1. Combine the original text and user edits to produce a more polished version.
2. Retain the original meaning while enhancing the structure, clarity, and tone.
3. Fix any spelling or grammatical errors.
4. Provide only the final version of the edited text.
5. Do not include any explanations, code, or extra characters in the output.

";

  public static readonly string ParaphrasePromptNormal = @"Instructions:
1. Paraphrase the given text while preserving the original meaning and tone.
2. Reword sentences to improve clarity and readability.
3. Keep the content concise and clear, without altering the overall message.
4. Do not add any new information, code, or extraneous characters.
5. Provide only the paraphrased text, without any explanations, comments, or other characters.
";
#endregion
    }
}
