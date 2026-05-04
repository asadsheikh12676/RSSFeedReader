 
**************************************************
specify init --here --ai copilot --script ps
**************************************************

This command specifies the following parameters:

	--here - Initializes GitHub Spec Kit in the current directory (your existing RSSFeedReader project).
	--ai copilot - Configures the project to use GitHub Copilot as the AI assistant.
	--script ps - Specifies that PowerShell scripts will be used.

The specify init command completes the following actions:

	Creates agent prompt files in the .github/agents/ and .github/prompts/ directories.
	Creates template files in the .specify/memory/ and .specify/templates/ directories.
	Creates script files in the .specify/scripts/powershell/ directory.
	Creates a settings.json file in the .vscode/ directory.
	Displays a success message (“Project ready”).
	Suggests some optional next steps.
	When you use the specify init command for a brownfield project, it recognizes that 
	the current directory isn’t empty and asks for confirmation before proceeding. 
	The command preserves any existing application files.

**********************************
SDD Projects - Default Structure
**********************************

 RSSFEEDREADER (root)
 ├── .github/
 │   ├── agents/                 (GitHub Spec Kit executable workflows that can be triggered via commands)
 │   └── prompts/                (GitHub Spec Kit prompt files that provide detailed instructions for each of the agent workflows)
 ├── .specify/                   (GitHub Spec Kit configuration)
 │   ├── memory/                 (GitHub Spec Kit stores the project constitution defining core principles and governance rules that all features must follow)
 │   ├── scripts/powershell/     (GitHub Spec Kit uses automation utilities (scripts) for creating features, setting up plans, and managing the specification workflow)
 │   └── templates/              (GitHub Spec Kit provides standardized markdown formats for specs, plans, tasks, and checklists to ensure consistent documentation across all features)
 └── .vscode/                    (Visual Studio Code configuration)


************************************
AI Models - Claude Vs ChatGPT Etc
************************************

IMPORTANT: This lab exercise was tested successfully using the GPT-5.2 and Claude Sonnet 4.5 models. 
Although both models were able to generate working applications, we did notice some differences. 
The Claude Sonnet 4.5 model tended to generate more detailed output. 

For example, the tasks.md file tended to have a larger number of tasks and phases. 
The Claude model’s responses were consistent and performance was reliable. 
The GPT-5.2 model tended to generate less detailed output. For example, a shorter list of more 
broadly scoped tasks, organized under fewer phases. The GPT model was able to implement tasks successfully, 
but might have used extra iterations to resolve bugs. 

The GPT model’s performance was generally good, but less consistent during our testing. 
For example, there were a couple times when the AI became unresponsive while processing a /speckit command. 
Restarting the command in the Chat view got things back on track quickly. 

Testing with older models, such as GPT-4 and GPT-5 mini, often generated unexpected results. 
If possible, we suggest using newer language models that are optimized for complex reasoning 
when running GitHub Spec Kit commands.