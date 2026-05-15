---
description: "Task list for RSS Feed Subscription Management (MVP) implementation"
---

# Tasks: RSS Feed Subscription Management (MVP)

**Input**: Design documents from `/specs/001-rss-subscriptions/`  
**Prerequisites**: plan.md ✓, spec.md ✓, data-model.md ✓, contracts/ ✓, quickstart.md ✓  
**Feature Branch**: `001-rss-subscriptions`  
**Tech Stack**: ASP.NET Core 8.0 Web API (backend) + Blazor WebAssembly (frontend)  
**Tests**: Not requested in specification — implementation only

**Organization**: Tasks organized by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2)
- Include exact file paths in descriptions

---

## Phase 1: Setup (Project Initialization)

**Purpose**: Create project structure and initialize .NET projects

- [x] T001 Create solution and project directories per implementation plan in backend/ and frontend/
- [x] T002 Create ASP.NET Core 8.0 Web API project in backend/RSSFeedReader.Api/RSSFeedReader.Api.csproj
- [x] T003 Create Blazor WebAssembly project in frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj
- [x] T004 Add both projects to solution file RSSFeedReader.sln
- [x] T005 Remove template demo pages from frontend/RSSFeedReader.UI/Pages/ (Home.razor, Counter.razor, Weather.razor)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before user story implementation

**⚠️ CRITICAL**: Both user stories depend on these foundational tasks

### Backend Infrastructure

- [x] T006 [P] Create Subscription model in backend/RSSFeedReader.Api/Models/Subscription.cs with Id, Url, AddedAt properties
- [x] T007 [P] Create SubscriptionService in backend/RSSFeedReader.Api/Services/SubscriptionService.cs with AddSubscription() and GetSubscriptions() methods
- [x] T008 Register SubscriptionService as singleton in backend/RSSFeedReader.Api/Program.cs
- [x] T009 Configure CORS policy in backend/RSSFeedReader.Api/Program.cs to allow frontend origin (localhost:5213)
- [x] T010 Update launchSettings.json in backend/RSSFeedReader.Api/Properties/ to use port 5151

### Frontend Infrastructure

- [x] T011 [P] Create SubscriptionApiClient service in frontend/RSSFeedReader.UI/Services/SubscriptionApiClient.cs with base URL pointing to http://localhost:5151/api
- [x] T012 [P] Create Subscription model in frontend/RSSFeedReader.UI/Models/Subscription.cs matching API contract
- [x] T013 Register SubscriptionApiClient in frontend/RSSFeedReader.UI/Program.cs
- [x] T014 Update launchSettings.json in frontend/RSSFeedReader.UI/Properties/ to use port 5213
- [x] T015 Create basic app.css in frontend/RSSFeedReader.UI/wwwroot/css/ with functional styling for forms and lists
- [x] T016 Update appsettings.json in frontend/RSSFeedReader.UI/wwwroot/ with ApiBaseUrl configuration

**Checkpoint**: Foundation ready - both user stories can now be implemented in parallel

---

## Phase 3: User Story 1 - Add Feed Subscription (Priority: P1) 🎯 MVP

**Goal**: Users can add feed subscriptions by entering a URL and clicking "Add Subscription"

**Independent Test**: User enters a feed URL, clicks Add Subscription, and sees the new subscription appear in the list immediately

### Implementation for User Story 1

- [x] T017 [US1] Create SubscriptionsController in backend/RSSFeedReader.Api/Controllers/SubscriptionsController.cs with POST endpoint
- [x] T018 [US1] Implement POST /api/subscriptions endpoint to accept URL and call SubscriptionService.AddSubscription()
- [x] T019 [US1] Return 201 Created response with Subscription object from POST endpoint (includes id, url, addedAt)
- [x] T020 [US1] Implement AddSubscriptionAsync() method in frontend/RSSFeedReader.UI/Services/SubscriptionApiClient.cs
- [x] T021 [US1] Create Subscriptions.razor page in frontend/RSSFeedReader.UI/Pages/Subscriptions.razor with @page "/"
- [x] T022 [US1] Add input field and Add Subscription button in Subscriptions.razor markup
- [x] T023 [US1] Implement OnAddSubscription() handler in Subscriptions.razor to call API and update local list
- [x] T024 [US1] Clear input field after successful subscription addition in Subscriptions.razor
- [x] T025 [US1] Add error message display in Subscriptions.razor for failed additions
- [x] T026 [US1] Update frontend App.razor routing to include Subscriptions page

**Checkpoint**: User Story 1 should be fully functional — users can add subscriptions via the form

---

## Phase 4: User Story 2 - Display Subscription List (Priority: P1) 🎯 MVP

**Goal**: Users see all subscriptions displayed in a list after adding them, with real-time updates

**Independent Test**: After adding one or more subscriptions, user sees a list showing all subscription URLs in order of addition

### Implementation for User Story 2

- [x] T027 [US2] Implement GET /api/subscriptions endpoint in SubscriptionsController.cs that returns List<Subscription>
- [x] T028 [US2] Implement GetSubscriptionsAsync() method in frontend/RSSFeedReader.UI/Services/SubscriptionApiClient.cs
- [x] T029 [US2] Load subscriptions on page initialization in Subscriptions.razor OnInitializedAsync()
- [x] T030 [US2] Add subscription list display in Subscriptions.razor showing all subscriptions with URL
- [x] T031 [US2] Create SubscriptionItem.razor component in frontend/RSSFeedReader.UI/Components/ to display individual subscription
- [x] T032 [US2] Update Subscriptions.razor to iterate over subscriptions and render SubscriptionItem for each
- [x] T033 [US2] Add empty state message in Subscriptions.razor when no subscriptions exist
- [x] T034 [US2] Add subscription count display in Subscriptions.razor (e.g., "Your Subscriptions (N total)")
- [x] T035 [US2] Ensure list updates immediately when new subscription is added (without page refresh)

**Checkpoint**: Both User Stories 1 and 2 complete — users can add and view subscriptions independently

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and documentation

- [x] T036 Verify backend project builds without errors (dotnet build backend/RSSFeedReader.Api/)
- [x] T037 Verify frontend project builds without errors (dotnet build frontend/RSSFeedReader.UI/)
- [x] T038 Verify solution builds without errors (dotnet build RSSFeedReader.sln)
- [x] T039 Run backend project and verify API responds on localhost:5151 (dotnet run in backend/)
- [x] T040 Run frontend project and verify UI loads on localhost:5213 (dotnet run in frontend/)
- [x] T041 Manually test User Story 1 (add subscription) via browser and Postman
- [x] T042 Manually test User Story 2 (display list) via browser
- [x] T043 Verify CORS configuration allows frontend requests to backend
- [x] T044 Validate subscription list renders without pagination for typical MVP usage (<50 items)
- [x] T045 Run quickstart.md validation steps to ensure local development setup works

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion — CRITICAL: BLOCKS both user stories
- **User Story 1 (Phase 3)**: Can start after Foundational is complete — Independent of US2
- **User Story 2 (Phase 4)**: Can start after Foundational is complete — Independent of US1 (but builds on foundation together)
- **Polish (Phase 5)**: Depends on user stories completion

### User Story Dependencies

- **User Story 1 & 2**: Both are P1 and equally critical — can run in parallel after Foundation
- No cross-story dependencies — each story is independently testable

### Within Each User Story

- Backend tasks before frontend consumption
- API endpoint before API client
- API client before page implementation
- Complete story before moving to next

### Parallel Opportunities

**Phase 1** (Setup):
- T002 (backend project) and T003 (frontend project) can run in parallel
- T005 (remove demo pages) can run after T003 completes

**Phase 2** (Foundational):
- All [P] tasks marked in backend can run in parallel (T006, T007)
- All [P] tasks marked in frontend can run in parallel (T011, T012)
- Backend and frontend infrastructure setups can run in parallel

**Phase 3 & 4** (User Stories):
- All tasks in User Story 1 can begin after Phase 2
- All tasks in User Story 2 can begin after Phase 2
- User Story 1 and 2 can run in parallel (separate backend and frontend tasks)

---

## Parallel Example: After Foundation (Phase 2) Complete

```
Developer A (Backend):
├── T017: Create SubscriptionsController
├── T018: Implement POST endpoint
└── T019: Return 201 response

Developer B (Frontend - Add):
├── T020: Implement AddSubscriptionAsync
├── T021: Create Subscriptions.razor page
├── T022: Add form markup
└── T023-T026: Add submission logic

Developer A (Backend):
├── T027: Implement GET endpoint
└── Complete User Story 2 backend

Developer B (Frontend - Display):
├── T028: Implement GetSubscriptionsAsync
├── T029-T035: Add list display & SubscriptionItem
└── Complete User Story 2 frontend
```

---

## Implementation Strategy

### MVP First (Both User Stories)

1. Complete Phase 1: Setup (5 tasks)
2. Complete Phase 2: Foundational (10 backend tasks + frontend tasks) — CRITICAL
3. Complete Phase 3: User Story 1 (9 tasks) — Users can add subscriptions
4. Complete Phase 4: User Story 2 (9 tasks) — Users can view subscriptions
5. Complete Phase 5: Polish (10 tasks) — Validate end-to-end
6. **MVP READY**: Deploy or demonstrate to stakeholders

### Incremental Delivery

Each phase completes a testable increment:
- **After Phase 2**: Foundation ready, no user-visible features yet
- **After Phase 3**: Users can add subscriptions (partial MVP)
- **After Phase 4**: Users can add AND view subscriptions (complete MVP)
- **After Phase 5**: MVP validated and tested end-to-end

### Sequential Team Strategy (Single Developer)

1. Complete all Phase 1 tasks
2. Complete all Phase 2 tasks
3. Complete all Phase 3 tasks — test User Story 1
4. Complete all Phase 4 tasks — test User Story 2
5. Complete all Phase 5 tasks — final validation

### Parallel Team Strategy (2-3 Developers)

1. **Team A + Team B**: Complete Phase 1 and Phase 2 together
2. Once Phase 2 done:
   - **Team A**: Phase 3 (backend add + frontend form)
   - **Team B**: Phase 4 (backend GET + frontend display)
   - **Team C** (if available): Polish/validation tasks in parallel
3. All teams converge for Phase 5 validation

---

## Task Completion Checklist

Use this to track progress through implementation:

**Phase 1**: ⬜⬜⬜⬜⬜  
**Phase 2**: ⬜⬜⬜⬜⬜⬜⬜⬜⬜⬜⬜  
**Phase 3**: ⬜⬜⬜⬜⬜⬜⬜⬜⬜  
**Phase 4**: ⬜⬜⬜⬜⬜⬜⬜⬜⬜  
**Phase 5**: ⬜⬜⬜⬜⬜⬜⬜⬜⬜⬜  

**Total**: 45 tasks across 5 phases

---

## Quality Gates

Before advancing to next phase, verify:

✓ **Phase 1 complete**: All projects created and added to solution  
✓ **Phase 2 complete**: Solution builds, both backend and frontend projects compile  
✓ **Phase 3 complete**: Backend POST endpoint works, frontend can call API and show results  
✓ **Phase 4 complete**: Backend GET endpoint works, frontend displays list of subscriptions  
✓ **Phase 5 complete**: End-to-end scenario works (add subscription → see in list immediately)

---

## Notes

- No tests required in MVP — specification does not request test tasks
- [P] tasks use different files and have no dependencies on other incomplete tasks
- Each task has specific file paths for clear implementation guidance
- Tasks are sized for 30-60 minutes each (typical development work unit)
- Commit progress after each phase or logical section (e.g., after T019 backend complete)
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence
- Verified against plan.md, spec.md, data-model.md, contracts/ — all references accurate
