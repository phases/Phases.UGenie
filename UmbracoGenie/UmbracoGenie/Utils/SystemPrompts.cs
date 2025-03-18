namespace Phases.UmbracoGenie
{
    public static class SystemPrompts
    {
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
    }
}
