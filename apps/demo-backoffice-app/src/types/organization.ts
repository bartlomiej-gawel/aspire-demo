export const OrganizationStatus = {
  Inactive: 1,
  Active: 2,
  Archived: 3
} as const

export type OrganizationStatus = typeof OrganizationStatus[keyof typeof OrganizationStatus]

export const OrganizationLocationStatus = {
  Inactive: 1,
  Active: 2,
  Archived: 3
} as const

export type OrganizationLocationStatus = typeof OrganizationLocationStatus[keyof typeof OrganizationLocationStatus]

export const OrganizationEmployeeStatus = {
  Inactive: 1,
  Invited: 2,
  Active: 3,
  Archived: 4
} as const

export type OrganizationEmployeeStatus = typeof OrganizationEmployeeStatus[keyof typeof OrganizationEmployeeStatus]

export const OrganizationEmployeeRole = {
  Admin: 1,
  Manager: 2,
  Employee: 3
} as const

export type OrganizationEmployeeRole = typeof OrganizationEmployeeRole[keyof typeof OrganizationEmployeeRole]

export const DayOfWeek = {
  Sunday: 0,
  Monday: 1,
  Tuesday: 2,
  Wednesday: 3,
  Thursday: 4,
  Friday: 5,
  Saturday: 6
} as const

export type DayOfWeek = typeof DayOfWeek[keyof typeof DayOfWeek]

export interface OrganizationLocationOpeningHoursRange {
  from: string
  to: string
  isEnabled: boolean
}

export interface OrganizationLocationOpeningHours {
  weekly: {
    [key in DayOfWeek]: OrganizationLocationOpeningHoursRange
  }
}

export interface OrganizationLocationAddress {
  country: string
  city: string
  street: string
  postalCode: string
}

export interface OrganizationLocation {
  id: string
  organizationId: string
  name: string
  openingHours: OrganizationLocationOpeningHours
  address: OrganizationLocationAddress | null
  status: OrganizationLocationStatus
  createdAt: string
  updatedAt: string | null
}

export interface OrganizationEmployee {
  id: string
  organizationId: string
  firstName: string
  lastName: string
  email: string
  phone: string | null
  status: OrganizationEmployeeStatus
  role: OrganizationEmployeeRole
  createdAt: string
  updatedAt: string | null
}

export interface Organization {
  id: string
  name: string
  status: OrganizationStatus
  createdAt: string
  updatedAt: string | null
  locations: OrganizationLocation[]
  employees: OrganizationEmployee[]
}
