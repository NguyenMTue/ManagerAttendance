# 🎯 PROJECT OVERVIEW: ATTENDANCE MANAGER WEB UI
**Objective:** Convert the existing Console-based Attendance Management System into a modern, responsive Web Application.

## 🛠️ TECH STACK & STRICT RULES
- **Framework:** Next.js (App Router), React, TypeScript.
- **Styling:** Tailwind CSS, Shadcn UI (for standard components like Modal, Table, Toast, Dropdown), Lucide React (for icons).
- **Data Fetching & State:** TanStack Query (React Query) v5, Axios/Fetch.
- **Forms & Validation:** React Hook Form, Zod.
- **Charts:** Recharts.
- **Rules for AI Agent:**
  1. Use strict TypeScript (`any` is not allowed). Define clear interfaces in `src/types/`.
  2. Follow Next.js App Router conventions. Use `"use client"` only when necessary (Hooks, Interactivity).
  3. All API calls must be wrapped in TanStack Query (`useQuery`, `useMutation`) with appropriate `invalidateQueries` on success.
  4. Create reusable UI components in `src/components/ui/` and feature components in `src/components/features/`.
  5. Implement RBAC (Role-Based Access Control) using Next.js Middleware to protect routes based on JWT claims (Admin, Manager, Employee).

---

## 📂 PROJECT STRUCTURE TARGET
```text
src/
├── app/
│   ├── (auth)/login/page.tsx
│   ├── (protected)/
│   │   ├── layout.tsx (Topbar + Sidebar)
│   │   ├── dashboard/page.tsx
│   │   ├── my-attendance/page.tsx
│   │   ├── attendance-management/page.tsx
│   │   ├── employees/page.tsx
│   │   └── employees/import-excel/page.tsx
├── components/
│   ├── layout/ (Sidebar, Topbar, UserNav)
│   ├── ui/ (Shadcn components)
│   └── features/ (Charts, DataTables, DynamicForms)
├── hooks/ (Custom hooks, useAuth, useAttendance, useEmployees)
├── lib/ (Axios instance, utils, endpoints)
└── types/ (TypeScript interfaces for API responses and models)

🚀 IMPLEMENTATION PHASES & TASKS
Phase 1: Setup & Authentication (Auth)
[ ] Initialize Next.js project with Tailwind & Shadcn UI.

[ ] Setup Axios instance with Request/Response Interceptors to handle JWT Bearer Token (inject token, handle 401).

[ ] Create /login page with React Hook Form + Zod validation.

[ ] Implement Auth Context / Zustand store to manage user state (Profile, Role).

[ ] Create Next.js middleware.ts to protect / routes and redirect unauthorized roles.

Phase 2: App Layout & Navigation
[ ] Create ProtectedLayout component.

[ ] Build Topbar: Display Company Logo, "Quick Check-in" badge status, and User Profile dropdown (Logout).

[ ] Build Sidebar:

Dynamic navigation based on Role (Employees don't see Employee Management).

Highlight active route.

Collapsible state.

Phase 3: Dashboard (/dashboard)
[ ] Build Stat Cards: Total active employees, Checked-in today, Late today (Red warning), Monthly attendance rate.

[ ] Integrate Recharts:

AttendanceStatusChart: Bar Chart (On-time vs Late by week/month).

DepartmentPieChart: Donut Chart (Dev, QA, Manager, etc.).

LateTrendChart: Line Chart (Late trends over the last 30 days).

Phase 4: Personal Attendance (/my-attendance)
[ ] Build Hero Card Widget:

Real-time Clock (UTC+7).

Status indicator (Not Checked-in, Checked-in at XX:XX, Checked-out).

Note input field.

Big 🟢 Check-In / 🔴 Check-Out Buttons. Use useMutation to handle actions.

[ ] Build Personal History Table: Columns for Date, In, Out, Work Hours, Status (Badge), Note. Add Date Range filters.

Phase 5: Attendance Management (/attendance-management) - Admin/Manager
[ ] Build Smart Filter Bar: Search by Name/Email, Filter by Department/Status, Date Picker.

[ ] Build Data Table: Display User info (Avatar + Name), Dept, Check-In, Check-Out, Status.

[ ] Add Action Column: View details. Admin gets a "Delete Record" button (triggers a Confirm Modal).

Phase 6: Employee Management (/employees) - Admin/Manager
[ ] Build Directory Table: Avatar, Name, Email, Type (Dev/QA/Manager badge), Department, Band/Level, Status.

[ ] Add Action Dropdown: Edit, Promote (Modal), Deactivate/Fire (Modal with Reason), Delete (Admin only).

[ ] Build Dynamic Form Modal (TPH Architecture) using React Hook Form:

Base fields: Name, Email, Password, Role.

Conditional rendering based on "Employee Type" select:

Developer: Show Technical Direction, Coding Skills.

QA: Show Testing Methodology, Automation Skills switch.

Manager: Show Manager Type, Managed Department.

Phase 7: Bulk Import Excel (/employees/import-excel)
[ ] Build Upload Zone: Drag & Drop area (react-dropzone) for .xlsx files. Add "Download Template" button.

[ ] Build Dry-Run Preview UI:

Summary Cards: Total Rows, Valid Rows (Green), Error Rows (Red).

Use Tabs (Shadcn Tabs) to switch between:

Tab 1 (Valid Rows): Table previewing users to be created.

Tab 2 (Errors): Table showing Excel Row | Error Column | Error Description | Invalid Data.

[ ] Build Confirmation Actions: "Cancel" and "Skip X errors & Save Y valid employees" (triggers final save mutation).

🔗 INTEGRATION NOTES FOR AI
When creating TanStack Query hooks, group them by feature domain (e.g., useGetEmployees, useCheckIn, useDryRunExcel).

For UI state that doesn't need to be in the URL, use useState. For filters (like search queries, date ranges), sync them with the URL using next/navigation (useSearchParams, useRouter).

Mock API calls temporarily using Promise.resolve() with dummy data if backend endpoints are not provided yet, but keep the Axios structure intact for easy swapping.