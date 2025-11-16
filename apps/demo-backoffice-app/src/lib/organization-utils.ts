import type { OrganizationStatus } from '@/types/organization'
import { OrganizationStatus as OrgStatus } from '@/types/organization'

export function getOrganizationStatusLabel(status: OrganizationStatus): string {
  switch (status) {
    case OrgStatus.Inactive:
      return 'Inactive'
    case OrgStatus.Active:
      return 'Active'
    case OrgStatus.Archived:
      return 'Archived'
    default:
      return 'Unknown'
  }
}

export function getOrganizationStatusVariant(
  status: OrganizationStatus,
): 'default' | 'secondary' | 'destructive' | 'outline' {
  switch (status) {
    case OrgStatus.Active:
      return 'default'
    case OrgStatus.Inactive:
      return 'secondary'
    case OrgStatus.Archived:
      return 'outline'
    default:
      return 'outline'
  }
}

export function formatDate(dateString: string | null): string {
  if (!dateString)
    return 'N/A'
  const date = new Date(dateString)
  return new Intl.DateTimeFormat('pl-PL', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  }).format(date)
}
