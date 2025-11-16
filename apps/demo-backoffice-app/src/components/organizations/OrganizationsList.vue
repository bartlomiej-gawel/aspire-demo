<script setup lang="ts">
import { ref } from 'vue'
import type { Organization, CreateOrganizationData } from '@/types/organization'
import { OrganizationStatus, OrganizationEmployeeStatus, OrganizationEmployeeRole, OrganizationLocationStatus } from '@/types/organization'
import { mockOrganizations } from '@/data/mock-organizations'
import OrganizationFormSheet from './OrganizationFormSheet.vue'
import {
  getOrganizationStatusLabel,
  getOrganizationStatusVariant,
  formatDate,
} from '@/lib/organization-utils'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Plus, Pencil } from 'lucide-vue-next'

const organizations = ref<Organization[]>(mockOrganizations)
const selectedOrganization = ref<Organization | null>(null)
const isDialogOpen = ref(false)

function handleCreateNew() {
  selectedOrganization.value = null
  isDialogOpen.value = true
}

function handleEdit(organization: Organization) {
  selectedOrganization.value = organization
  isDialogOpen.value = true
}

function isCreateOrganizationData(data: any): data is CreateOrganizationData {
  return 'organizationName' in data && 'organizationAdmins' in data
}

function handleSave(data: Partial<Organization> | CreateOrganizationData) {
  if (isCreateOrganizationData(data)) {
    // Create new organization from backoffice
    const organizationId = crypto.randomUUID()
    const now = new Date().toISOString()

    const newOrganization: Organization = {
      id: organizationId,
      name: data.organizationName,
      status: OrganizationStatus.Inactive,
      createdAt: now,
      updatedAt: null,
      locations: [
        {
          id: crypto.randomUUID(),
          organizationId,
          name: `${data.organizationName} Headquarters`,
          openingHours: {
            weekly: {
              0: { from: '09:00', to: '17:00', isEnabled: false },
              1: { from: '09:00', to: '17:00', isEnabled: true },
              2: { from: '09:00', to: '17:00', isEnabled: true },
              3: { from: '09:00', to: '17:00', isEnabled: true },
              4: { from: '09:00', to: '17:00', isEnabled: true },
              5: { from: '09:00', to: '17:00', isEnabled: true },
              6: { from: '10:00', to: '14:00', isEnabled: true },
            },
          },
          address: null,
          status: OrganizationLocationStatus.Active,
          createdAt: now,
          updatedAt: null,
        },
      ],
      employees: data.organizationAdmins.map(admin => ({
        id: crypto.randomUUID(),
        organizationId,
        firstName: admin.firstName,
        lastName: admin.lastName,
        email: admin.email,
        phone: admin.phone,
        status: OrganizationEmployeeStatus.Inactive,
        role: OrganizationEmployeeRole.Admin,
        createdAt: now,
        updatedAt: null,
      })),
    }

    organizations.value.push(newOrganization)
  }
  else if (data.id) {
    // Edit existing organization
    organizations.value = organizations.value.map(org =>
      org.id === data.id
        ? {
            ...org,
            ...data,
            updatedAt: new Date().toISOString(),
          }
        : org,
    )
  }
}
</script>

<template>
  <div class="flex flex-1 flex-col gap-4">
    <div class="flex items-center justify-between">
      <div>
        <h2 class="text-2xl font-bold tracking-tight">
          Organizations
        </h2>
        <p class="text-muted-foreground">
          Manage organizations in the system
        </p>
      </div>
      <Button @click="handleCreateNew">
        <Plus class="mr-2 h-4 w-4" />
        Add Organization
      </Button>
    </div>

    <div class="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Name</TableHead>
            <TableHead>Status</TableHead>
            <TableHead>Locations</TableHead>
            <TableHead>Employees</TableHead>
            <TableHead>Created At</TableHead>
            <TableHead>Updated At</TableHead>
            <TableHead class="w-[100px]">
              Actions
            </TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          <TableRow v-if="organizations.length === 0">
            <TableCell colspan="7" class="text-center text-muted-foreground">
              No organizations found
            </TableCell>
          </TableRow>
          <TableRow v-for="organization in organizations" :key="organization.id">
            <TableCell class="font-medium">
              {{ organization.name }}
            </TableCell>
            <TableCell>
              <Badge :variant="getOrganizationStatusVariant(organization.status)">
                {{ getOrganizationStatusLabel(organization.status) }}
              </Badge>
            </TableCell>
            <TableCell>{{ organization.locations.length }}</TableCell>
            <TableCell>{{ organization.employees.length }}</TableCell>
            <TableCell class="text-muted-foreground">
              {{ formatDate(organization.createdAt) }}
            </TableCell>
            <TableCell class="text-muted-foreground">
              {{ formatDate(organization.updatedAt) }}
            </TableCell>
            <TableCell>
              <Button
                variant="ghost"
                size="sm"
                @click="handleEdit(organization)"
              >
                <Pencil class="h-4 w-4" />
              </Button>
            </TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </div>

    <OrganizationFormSheet
      v-model:open="isDialogOpen"
      :organization="selectedOrganization"
      @save="handleSave"
    />
  </div>
</template>
