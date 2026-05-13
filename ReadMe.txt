 
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


************************************************
Constituion.md file and it's considerations
************************************************

You should review the constitution to ensure it captures requirements accurately. 

This step is important when you’re working in a production environment where the constitution represents your business requirements and technical governance. 

For a training exercise, this review is mainly to help you become familiar with the constitution content.

In the constitution file, each principle should be clearly stated and actionable. For example:

Vague: “Apply security best practices.” is too general.
Clear: “All API endpoints MUST validate inputs before processing (URL format validation, length limits, null checks).” is specific and actionable.

If critical requirements are missing or unclear, you can edit the constitution.md file directly to add or modify principles.

In a production scenario, it’s important to review the constitution against the following criteria:

> Completeness: All major areas are covered.
> Clarity: Each principle is specific and unambiguous.
> Consistency: Principles don’t contradict each other.
> Relevance: All principles relate to the RSSFeedReader project.

************************************************
Spec.md file and it's considerations
************************************************

The specification (spec.md) defines what you’re building from the user’s perspective. It describes the features, user stories, acceptance criteria, and business requirements without prescribing how to implement them. 

A well-written spec serves as the foundation for creating the implementation plan and tasks.


Notice that GitHub Copilot retains the context of previous interactions in the current chat session. If you generated the constitution.md file in the current session, GitHub Copilot provides a Build Specification button near the bottom of the Chat view that could be used to start generating the specification. 

In our test case, you want to provide the requirements document explicitly, so you don’t use the Build Specification button.


************************************************
Plan.md file and it's considerations
************************************************

The technical plan bridges the gap between the “what” (specification) and the “how” (implementation). 

It defines the architecture, technology choices, data models, API designs, and implementation approach while adhering to the constraints defined in the constitution

> The plan-template.md file defines the structure and sections of a technical plan document.

> The speckit.plan.agent.md file provides detailed instructions for the /speckit.plan command. It guides GitHub Copilot on how to create a technical plan based on the specification and constitution.


Once the plan workflow is complete, the following files are normally added to the root of the specs folder:

> plan.md		(outlines the technical implementation plan for the application)

> research.md		(captures research findings and technology decisions for the application)

> quickstart.md		(provides setup instructions and a high-level overview of how to get started with the 				implementation. In case, the tasks need to be done MANUALLY; this file become super 				important for the architect and developers)

> data-model.md		(defines the data entities, properties, and relationships needed for the application)

> Contracts Folder:	(There could be one or more files listed under a contracts folder. 
			Like API Contracts, and Front End application design summary etc)

IMPORTANT: 
> For a production scenario, you need to ensure that the plan provides a comprehensive description of the technical context and a clearly defined implementation strategy for the new app/features. 

> The research, quickstart, and data model files should complement the plan by providing additional context and details.


***************************************
Task.md file and it's considerations
***************************************

The tasks.md file breaks down the technical plan into specific, actionable implementation steps. 

IMPORTANT:
> Each task should be small enough to complete in a reasonable timeframe (typically a few hours to a day when implemented without AI assistance) 

> Each task should have a clear acceptance criteria.

> The file tasks-template.md file organizes tasks into logical phases, while the speckit.tasks.agent.md file describes the steps that the /speckit.tasks workflow should follow:

	What inputs to read (spec.md, plan.md, etc.)
	What to produce (tasks.md)
	How to sequence the tasks (by phase, user story, etc.)
	How to define each task (specific, actionable, testable)
	What checks/gates to apply (coverage, ordering, scope)

AFTER THE speckit/tasks command has been completed successfully, VERIFY THE FOLLOWING

1) Verify that tasks are ordered logically by phase and user story. For example:

> Setup and Foundation tasks come first.

> Backend API tasks build on the foundation.

> Frontend tasks reference backend endpoints.

> Testing tasks come after implementation.

> Deployment tasks come last.

IMPORTANT:

PROD SCENARIO: ou should also take the time to verify that every requirement (from spec.md) and every key design commitment (from plan.md) maps to at least one concrete task (usually several). For example:

> Design commitments from the plan.md file should have corresponding implementation tasks.

> User story acceptance criteria should have corresponding implementation and verification tasks.

> Functional requirements should have corresponding implementation tasks.

> Security requirements should have corresponding implementation tasks.

> Performance requirements should have testing tasks.




