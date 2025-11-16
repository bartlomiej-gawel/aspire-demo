<script setup lang="ts">
import { ref } from 'vue'
import type { Organization } from '@/types/organization'
import { mockOrganizations } from '@/data/mock-organizations'
import OrganizationFormDialog from './OrganizationFormDialog.vue'
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

function handleSave(organizationData: Partial<Organization>) {
  if (organizationData.id) {
    // Edit existing
    organizations.value = organizations.value.map(org =>
      org.id === organizationData.id
        ? {
            ...org,
            ...organizationData,
            updatedAt: new Date().toISOString(),
          }
        : org,
    )
  }
  else {
    // Create new
    const newOrganization: Organization = {
      id: crypto.randomUUID(),
      name: organizationData.name!,
      status: organizationData.status!,
      createdAt: new Date().toISOString(),
      updatedAt: null,
      locations: [],
      employees: [],
    }
    organizations.value.push(newOrganization)
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

    <OrganizationFormDialog
      v-model:open="isDialogOpen"
      :organization="selectedOrganization"
      @save="handleSave"
    />
  </div>
</template>
